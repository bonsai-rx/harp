using Bonsai;
using System;
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

        StateType state = StateType.Active; // Default value
        LedType state_led = LedType.On;     // Default value
        LedType visual = LedType.On;        // Default value


        [Description("Select device COM port.")]
        [TypeConverter(typeof(PortNameConverter))]
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;

                // This code is run (asynchronously) only when the port name changes
                ConfAndGetDeviceName(portName, state_led, visual).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Defines the State of the Device during run time.")]
        public StateType State
        {
            get { return state; }

            set
            {
                state = value;
            }
        }
        
        [Description("Defines if the Device's LED that indicates current State is On or Off.")]
        public LedType State_LED
        {
            get { return state_led; }

            set
            {
                state_led = value;

                // This code is run (asynchronously) only when the state led changes
                ConfAndGetDeviceName(portName, state_led, visual).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Defines if all the visual indicators of the Device are On or Off.")]
        public LedType Visual
        {
            get { return visual; }

            set
            {
                visual = value;

                // This code is run (asynchronously) only when the visual changes
                ConfAndGetDeviceName(portName, state_led, visual).Subscribe(deviceName => name = deviceName);
            }
        }

        static IObservable<string> ConfAndGetDeviceName(string portName, LedType state_led, LedType visual)
        {
            
            return Observable.Start(() =>
            {
                byte checksumReadWhoAmI = 1 + 4 + 0 + 255 + 0x02 - 256;
                var cmdReadWhoAmI = new byte[] { 1, 4, 0, 255, 0x02, checksumReadWhoAmI};
                var replyReadWhoAmI = new byte[8];

                byte checksumWriteOpCtrl = 2 + 5 + 10 + 0x01 + 255 - 256;
                var cmdWriteOpCtrl = new byte[] { 2, 5, 10, 255, 0x01, 0, checksumWriteOpCtrl};
                var replyWriteOpCtrl = new byte[5];

                cmdWriteOpCtrl[5]  = (state_led == LedType.On) ? (byte)0x40 : (byte)0;
                cmdWriteOpCtrl[5] += (visual == LedType.On) ? (byte)0x20 : (byte)0;
                checksumWriteOpCtrl += cmdWriteOpCtrl[5];
                cmdWriteOpCtrl[6] = checksumWriteOpCtrl;

                using (var devicePort = new SerialPort(portName, 2000000, Parity.None, 8, StopBits.One))
                {
                    try
                    {
                        devicePort.Open();
                        devicePort.ReadTimeout = 10000;

                        devicePort.Write(cmdWriteOpCtrl, 0, cmdWriteOpCtrl.Length);
                        devicePort.RtsEnable = true;
                        devicePort.Read(replyWriteOpCtrl, 0, replyWriteOpCtrl.Length);
                        devicePort.RtsEnable = false;

                        devicePort.Write(cmdReadWhoAmI, 0, cmdReadWhoAmI.Length);
                        devicePort.RtsEnable = true;
                        devicePort.Read(replyReadWhoAmI, 0, replyReadWhoAmI.Length);

                        switch (BitConverter.ToUInt16(replyReadWhoAmI, 5))
                        {
                            case 1104:
                                return ("Synchronizer (" + portName + ")");
                            default:
                                return "NotSpecified";
                        }
                        //return reply[4].ToString;
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
                transport.Open();

                byte checksumWriteOpCtrl = 2 + 5 + 10 + 0x01 + 255 - 256;
                var cmdWriteOpCtrl = new byte[] { 2, 5, 10, 255, 0x01, 0, checksumWriteOpCtrl };
                var replyWriteOpCtrl = new byte[5];

                cmdWriteOpCtrl[5] = (state_led == LedType.On) ? (byte)0x40 : (byte)0;
                cmdWriteOpCtrl[5] += (visual == LedType.On) ? (byte)0x20 : (byte)0;
                cmdWriteOpCtrl[5] += (state == StateType.Active) ? (byte)0x01 : (byte)0;
                checksumWriteOpCtrl += cmdWriteOpCtrl[5];
                cmdWriteOpCtrl[6] = checksumWriteOpCtrl;

                transport.Write(new HarpDataFrame(cmdWriteOpCtrl));
                var cleanup = Disposable.Create(() =>
                {
                    Console.WriteLine("!");
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
                transport.Open();
                

                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Do(
                    input => transport.Write(input),
                    ex => {
                        observer.OnError(ex);
                    },
                    () =>
                    {
                        observer.OnCompleted(); 
                    }
                    
                    ).Subscribe();

                return new CompositeDisposable(
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
