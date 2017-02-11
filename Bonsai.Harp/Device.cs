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
    public class Device : Source<HarpDataFrame>, INamedElement
    {
        string name;
        string portName;
        LedType ledState;
        LedType visualIndicators;

        public Device()
        {
            PortName = "COMx";
            DeviceState = StateType.Active;
            LedState = LedType.On;
            VisualIndicators = LedType.On;
            ReadAllRegisters = true;
        }


        [Description("Select device COM port. (This option can't be change during run mode)")]
        [TypeConverter(typeof(PortNameConverter))]
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

        [Description("Defines the State of the Device during run time. (This option can't be change during run time)")]
        public StateType DeviceState { get; set; }

        [Description("Defines if the device will send the content of all values when go to run time. (This option can't be change during run time)")]
        public bool ReadAllRegisters { get; set; }

        [Description("Defines if the Device's LED that indicates current State is On or Off. (This option can't be change during run time)")]
        public LedType LedState
        {
            get { return ledState; }

            set
            {
                ledState = value;

                // This code is run (asynchronously) only when the state led changes
                ConfAndGetDeviceName(portName, ledState, visualIndicators).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Defines if all the visual indicators of the Device are On or Off. (This option can't be change during run time)")]
        public LedType VisualIndicators
        {
            get { return visualIndicators; }
            set
            {
                visualIndicators = value;

                // This code is run (asynchronously) only when the visual changes
                ConfAndGetDeviceName(portName, ledState, visualIndicators).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Ignore errors")]
        public bool IgnoreErrors { get; set; }

        static byte[] CreateWriteOpCtrlCmd(StateType stateMode, LedType operationLED, LedType visualEN, bool dumpRegisters)
        {
            byte checksumWriteOpCtrl = 2 + 5 + 10 + 0x01 + 255 - 256;
            var cmdWriteOpCtrl = new byte[] { 2, 5, 10, 255, 0x01, 0, checksumWriteOpCtrl };

            cmdWriteOpCtrl[5] = (operationLED == LedType.On) ? (byte)0x40 : (byte)0;
            cmdWriteOpCtrl[5] += (visualEN == LedType.On) ? (byte)0x20 : (byte)0;
            cmdWriteOpCtrl[5] += dumpRegisters ? (byte)0x08 : (byte)0;
            cmdWriteOpCtrl[5] += (stateMode == StateType.Active) ? (byte)0x01 : (byte)0;
            checksumWriteOpCtrl += cmdWriteOpCtrl[5];
            cmdWriteOpCtrl[6] = checksumWriteOpCtrl;

            return cmdWriteOpCtrl;
        }

        static IObservable<string> ConfAndGetDeviceName(string portName, LedType state_led, LedType visual)
        {
            
            return Observable.Start(() =>
            {
                byte checksumReadWhoAmI = 1 + 4 + 0 + 255 + 0x02 - 256;
                var cmdReadWhoAmI = new byte[] { 1, 4, 0, 255, 0x02, checksumReadWhoAmI };
                var replyReadWhoAmI = new byte[14];

                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(StateType.Standby, state_led, visual, false);
                var replyWriteOpCtrl = new byte[13];

                using (var devicePort = new SerialPort(portName, 2000000, Parity.None, 8, StopBits.One))
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
                                return ("Poke (" + portName + ")");
                            case 1040:
                                return ("MultiPwmGenerator (" + portName + ")");
                            case 1056:
                                return ("WearBasestation (" + portName + ")");
                            case 1072:
                                return ("12V Output Drive (" + portName + ")");
                            case 1088:
                                return ("LED array (" + portName + ")");
                            case 1104:
                                return ("Synchronizer (" + portName + ")");

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


        public override IObservable<HarpDataFrame> Generate()
        {
            return Observable.Create<HarpDataFrame>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.IgnoreErrors = IgnoreErrors;
                transport.Open();
                
                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState, ledState, visualIndicators, ReadAllRegisters);
                transport.Write(new HarpDataFrame(cmdWriteOpCtrl));

                var cleanup = Disposable.Create(() =>
                {
                    //Console.WriteLine("!");
                    cmdWriteOpCtrl = CreateWriteOpCtrlCmd(StateType.Standby, ledState, visualIndicators, false);
                    transport.Write(new HarpDataFrame(cmdWriteOpCtrl));
                });

                return new CompositeDisposable(
                    cleanup,
                    transport);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<HarpDataFrame> source)
        {
            return Observable.Create<HarpDataFrame>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.IgnoreErrors = IgnoreErrors;
                transport.Open();

                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState, ledState, visualIndicators, ReadAllRegisters);
                transport.Write(new HarpDataFrame(cmdWriteOpCtrl));

                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Do(
                    input => transport.Write(input),
                    ex => {
                        //observer.OnError(ex);
                        cmdWriteOpCtrl = CreateWriteOpCtrlCmd(StateType.Standby, ledState, visualIndicators, false);
                        transport.Write(new HarpDataFrame(cmdWriteOpCtrl));
                    },
                    () =>
                    {
                        //observer.OnCompleted();
                        cmdWriteOpCtrl = CreateWriteOpCtrlCmd(StateType.Standby, ledState, visualIndicators, false);
                        transport.Write(new HarpDataFrame(cmdWriteOpCtrl));
                    }

                    ).Subscribe();

                var cleanup = Disposable.Create(() =>
                {
                    //Console.WriteLine("!");
                    cmdWriteOpCtrl = CreateWriteOpCtrlCmd(StateType.Standby, ledState, visualIndicators, false);
                    transport.Write(new HarpDataFrame(cmdWriteOpCtrl));
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
