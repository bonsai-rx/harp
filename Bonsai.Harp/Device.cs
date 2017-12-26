using Bonsai;
using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Bonsai.Harp
{
    [Description("Produces a sequence of messages from the Harp device connected at the specified serial port.")]
    public class Device : Source<HarpMessage>, INamedElement
    {
        string name;
        string portName;
        LedState ledState;
        LedState visualIndicators;

        public Device()
        {
            PortName = "COMx";
            DeviceState = DeviceState.Active;
            LedState = LedState.On;
            VisualIndicators = LedState.On;
            ReadAllRegisters = true;
        }


        [TypeConverter(typeof(PortNameConverter))]
        [Description("The name of the serial port used to communicate with the Harp device.")]
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;

                // This code is run (asynchronously) only when the port name changes
                ConfAndGetDeviceName(portName, ledState, visualIndicators).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Specifies the state of the device at run time.")]
        public DeviceState DeviceState { get; set; }

        [Description("Specifies whether the device should send the content of all registers during initialization.")]
        public bool ReadAllRegisters { get; set; }

        [Description("Specifies the state of the device LED.")]
        public LedState LedState
        {
            get { return ledState; }

            set
            {
                ledState = value;

                // This code is run (asynchronously) only when the state led changes
                ConfAndGetDeviceName(portName, ledState, visualIndicators).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Specifies the state of all the visual indicators in the device.")]
        public LedState VisualIndicators
        {
            get { return visualIndicators; }
            set
            {
                visualIndicators = value;

                // This code is run (asynchronously) only when the visual changes
                ConfAndGetDeviceName(portName, ledState, visualIndicators).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Specifies whether error messages parsed during acquisition should be ignored.")]
        public bool IgnoreErrors { get; set; }

        static byte[] CreateWriteOpCtrlCmd(DeviceState stateMode, LedState operationLED, LedState visualEN, bool dumpRegisters)
        {
            byte checksumWriteOpCtrl = 2 + 5 + 10 + 0x01 + 255 - 256;
            var cmdWriteOpCtrl = new byte[] { 2, 5, 10, 255, 0x01, 0, checksumWriteOpCtrl };

            cmdWriteOpCtrl[5] = (operationLED == LedState.On) ? (byte)0x40 : (byte)0;
            cmdWriteOpCtrl[5] += (visualEN == LedState.On) ? (byte)0x20 : (byte)0;
            cmdWriteOpCtrl[5] += dumpRegisters ? (byte)0x08 : (byte)0;
            cmdWriteOpCtrl[5] += (stateMode == DeviceState.Active) ? (byte)0x01 : (byte)0;
            checksumWriteOpCtrl += cmdWriteOpCtrl[5];
            cmdWriteOpCtrl[6] = checksumWriteOpCtrl;

            return cmdWriteOpCtrl;
        }

        static IObservable<string> ConfAndGetDeviceName(string portName, LedState state_led, LedState visual)
        {
            return Observable.Start(() =>
            {
                byte checksumReadWhoAmI = 1 + 4 + 0 + 255 + 0x02 - 256;
                var cmdReadWhoAmI = new byte[] { 1, 4, 0, 255, 0x02, checksumReadWhoAmI };
                var replyReadWhoAmI = new byte[14];

                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState.Standby, state_led, visual, false);
                var replyWriteOpCtrl = new byte[13];

                //using (var devicePort = new SerialPort(portName, 2000000, Parity.None, 8, StopBits.One))
                using (var devicePort = new SerialPort(portName, 1000000, Parity.None, 8, StopBits.One))
                {
                    try
                    {
                        devicePort.Handshake = Handshake.RequestToSend;
                        devicePort.Open();

                        devicePort.Write(cmdWriteOpCtrl, 0, cmdWriteOpCtrl.Length);
                        devicePort.Write(cmdReadWhoAmI, 0, cmdReadWhoAmI.Length);

                        int timeout_ms = 500;
                        while ((devicePort.BytesToRead < 13 + 14) && timeout_ms > 0)
                        {
                            Thread.Sleep(1);
                            timeout_ms--;
                        }

                        int a = devicePort.Read(replyWriteOpCtrl, 0, 13);
                        int b = devicePort.Read(replyReadWhoAmI, 0, 14);

                        switch (BitConverter.ToUInt16(replyReadWhoAmI, 11))
                        {
                            case 1024:
                                return ("Poke");
                            case 1040:
                                return ("MultiPwm");
                            case 1056:
                                return ("WearBasestation");
                            case 1072:
                                return ("12VoltsDrive");
                            case 1088:
                                return ("LedController");
                            case 1104:
                                return ("Synchronizer");
                            case 1121:
                                return ("SimpleAnalogGenerator");
                            case 1136:
                                return ("Arquimedes");
                            case 1152:
                                return ("ClockSynchronizer");
                            case 1168:
                                return ("Camera");
                            case 1184:
                                return ("PyControl");
                            case 1216:
                                return ("Behavior");
                            default:
                                return "NotSpecified";
                        }
                    }
                    catch
                    {
                        return typeof(Device).Name;
                    }
                }
            });
        }


        public override IObservable<HarpMessage> Generate()
        {
            return Observable.Create<HarpMessage>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.IgnoreErrors = IgnoreErrors;
                transport.Open();
                
                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState, ledState, visualIndicators, ReadAllRegisters);
                transport.Write(new HarpMessage(cmdWriteOpCtrl));

                var cleanup = Disposable.Create(() =>
                {
                    //Console.WriteLine("!");
                    cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState.Standby, ledState, visualIndicators, false);
                    transport.Write(new HarpMessage(cmdWriteOpCtrl));
                });

                return new CompositeDisposable(
                    cleanup,
                    transport);
            });
        }

        public IObservable<HarpMessage> Generate(IObservable<HarpMessage> source)
        {
            return Observable.Create<HarpMessage>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.IgnoreErrors = IgnoreErrors;
                transport.Open();

                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState, ledState, visualIndicators, ReadAllRegisters);
                transport.Write(new HarpMessage(cmdWriteOpCtrl));

                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Subscribe(
                    transport.Write,
                    observer.OnError,
                    observer.OnCompleted);

                var cleanup = Disposable.Create(() =>
                {
                    //Console.WriteLine("!");
                    cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState.Standby, ledState, visualIndicators, false);
                    transport.Write(new HarpMessage(cmdWriteOpCtrl));
                });

                return new CompositeDisposable(
                    cleanup,
                    sourceDisposable,
                    transport);
            });
        }

        string INamedElement.Name
        {
            get { return !string.IsNullOrEmpty(name) ? name : typeof(Device).Name; }
        }
    }
}
