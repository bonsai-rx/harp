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
        EnableType heartbeat;
        EnableType replies;

        public Device()
        {
            PortName = "COMx";
            DeviceState = DeviceState.Active;
            LedState = LedState.On;
            VisualIndicators = LedState.On;
            DumpRegisters = true;
            Heartbeat = EnableType.Disable;
            CommandReplies = EnableType.Enable;
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
                ConfAndGetDeviceName(portName, ledState, visualIndicators, heartbeat).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Specifies the state of the device at run time.")]
        public DeviceState DeviceState { get; set; }

        [Description("Specifies whether the device should send the content of all registers during initialization.")]
        public bool DumpRegisters { get; set; }

        [Description("Specifies the state of the device LED.")]
        public LedState LedState
        {
            get { return ledState; }

            set
            {
                ledState = value;

                // This code is run (asynchronously) only when the state led changes
                ConfAndGetDeviceName(portName, ledState, visualIndicators, heartbeat).Subscribe(deviceName => name = deviceName);
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
                ConfAndGetDeviceName(portName, ledState, visualIndicators, heartbeat).Subscribe(deviceName => name = deviceName);
            }
        }

        [Description("Specifies if the Device sends the Timestamp Event each second.")]
        public EnableType Heartbeat
        {
            get { return heartbeat; }

            set
            {
                heartbeat = value;
            }
        }

        [Description("Specifies if the Device replies to commands.")]
        EnableType CommandReplies
        {
            get { return replies; }

            set
            {
                replies = value;
            }
        }

        [Description("Specifies whether error messages parsed during acquisition should be ignored or create an exception.")]
        public bool IgnoreErrors { get; set; }        

        static byte[] CreateWriteOpCtrlCmd(DeviceState stateMode, LedState operationLED, LedState visualEN, EnableType heartbeatEn, EnableType cmdReplies, bool dumpRegisters)
        {
            byte checksumWriteOpCtrl = 2 + 5 + 10 + 0x01 + 255 - 256;
            var cmdWriteOpCtrl = new byte[] { 2, 5, 10, 255, 0x01, 0, checksumWriteOpCtrl };

            cmdWriteOpCtrl[5]    = (heartbeatEn == EnableType.Enable) ? (byte)0x80 : (byte)0;
            cmdWriteOpCtrl[5]   += (operationLED == LedState.On)      ? (byte)0x40 : (byte)0;
            cmdWriteOpCtrl[5]   += (visualEN == LedState.On)          ? (byte)0x20 : (byte)0;
            cmdWriteOpCtrl[5]   += (cmdReplies == EnableType.Enable)  ? (byte)0    : (byte)0x10;
            cmdWriteOpCtrl[5]   += dumpRegisters                      ? (byte)0x08 : (byte)0;
            cmdWriteOpCtrl[5]   += (stateMode == DeviceState.Active)  ? (byte)0x01 : (byte)0;
            checksumWriteOpCtrl += cmdWriteOpCtrl[5];
            cmdWriteOpCtrl[6]    = checksumWriteOpCtrl;

            return cmdWriteOpCtrl;
        }

        static IObservable<string> ConfAndGetDeviceName(string portName, LedState state_led, LedState visual, EnableType heartbeatEn)
        {
            return Observable.Start(() =>
            {
                /* Commands */
                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState.Standby, state_led, visual, heartbeatEn, EnableType.Enable, false);
                var cmdReadWhoAmI               = new HarpMessage(true, 1, 4, 0,  255, (byte)PayloadType.U16, 0);
                var cmdReadMajorHardwareVersion = new HarpMessage(true, 1, 4, 1,  255, (byte)PayloadType.U8,  0);
                var cmdReadMinorHardwareVersion = new HarpMessage(true, 1, 4, 2,  255, (byte)PayloadType.U8,  0);
                var cmdReadMajorFirmwareVersion = new HarpMessage(true, 1, 4, 6,  255, (byte)PayloadType.U8,  0);
                var cmdReadMinorFirmwareVersion = new HarpMessage(true, 1, 4, 7,  255, (byte)PayloadType.U8,  0);
                var cmdReadTimestampSeconds     = new HarpMessage(true, 1, 4, 8,  255, (byte)PayloadType.U32, 0);
                var cmdReadDeviceName           = new HarpMessage(true, 1, 4, 12, 255, (byte)PayloadType.U8,  0);

                /* Buffers to receive replies */
                var rplWriteOpCtrl              = new byte[12 + 1];
                var rplReadWhoAmI               = new byte[12 + 2];
                var rplReadMajorHardwareVersion = new byte[12 + 1];
                var rplReadMinorHardwareVersion = new byte[12 + 1];
                var rplReadMajorFirmwareVersion = new byte[12 + 1];
                var rplReadMinorFirmwareVersion = new byte[12 + 1];
                var rplReadTimestampSeconds     = new byte[12 + 4];
                var rplReadDeviceName           = new byte[12 + 25];
                
                using (var devicePort = new SerialPort(portName, 1000000, Parity.None, 8, StopBits.One))
                {
                    try
                    {
                        /* Open COM port */
                        devicePort.Handshake = Handshake.RequestToSend;
                        devicePort.Open();

                        /************************************************************************/
                        /* Write Operation Control register                                     */
                        /************************************************************************/
                        devicePort.Write(cmdWriteOpCtrl, 0, cmdWriteOpCtrl.Length);

                        /* Wait until all bytes are available or until timeout is achieved */
                        int timeout_ms = 500;
                        while ((devicePort.BytesToRead < rplWriteOpCtrl.Length) && timeout_ms > 0)
                        {
                            Thread.Sleep(1);

                            if (--timeout_ms == 0)
                            {
                                if (devicePort.BytesToRead == 0)
                                    Console.WriteLine("Serial Harp device didn't reply when writing to Operation Control registers.");
                                else
                                    Console.WriteLine("Serial Harp device didn't reply correctly when writing to Operatin Control registers.");

                                return ("Device");
                            }
                        }

                        int replySize;
                        replySize = devicePort.Read(rplWriteOpCtrl, 0, rplWriteOpCtrl.Length);

                        /************************************************************************/
                        /* Read all mandatory registers                                         */
                        /************************************************************************/
                        devicePort.Write(cmdReadWhoAmI.MessageBytes,               0, cmdReadWhoAmI.MessageBytes.Length);
                        devicePort.Write(cmdReadMajorHardwareVersion.MessageBytes, 0, cmdReadWhoAmI.MessageBytes.Length);
                        devicePort.Write(cmdReadMinorHardwareVersion.MessageBytes, 0, cmdReadWhoAmI.MessageBytes.Length);
                        devicePort.Write(cmdReadMajorFirmwareVersion.MessageBytes, 0, cmdReadWhoAmI.MessageBytes.Length);
                        devicePort.Write(cmdReadMinorFirmwareVersion.MessageBytes, 0, cmdReadWhoAmI.MessageBytes.Length);
                        devicePort.Write(cmdReadTimestampSeconds.MessageBytes,     0, cmdReadTimestampSeconds.MessageBytes.Length);

                        /* Calculate the number of bytes to read */
                        int bytesToRead = 0;
                        bytesToRead += rplReadWhoAmI.Length + rplReadMajorHardwareVersion.Length + rplReadMinorHardwareVersion.Length;
                        bytesToRead += rplReadMajorFirmwareVersion.Length + rplReadMinorFirmwareVersion.Length + rplReadTimestampSeconds.Length;

                        /* Wait until all bytes are available or until timeout is achieved */
                        timeout_ms = 500;
                        while ((devicePort.BytesToRead < bytesToRead) && timeout_ms > 0)
                        {
                            Thread.Sleep(1);

                            if (--timeout_ms == 0)
                            {
                                if (devicePort.BytesToRead == 0)
                                    Console.WriteLine("Serial Harp device didn't reply when reading the mandatory registers.");
                                else
                                    Console.WriteLine("Serial Harp device didn't reply correctly when reading the mandatory registers.");

                                return ("Device");
                            }
                        }

                        /* Receive the replies */
                        replySize = devicePort.Read(rplReadWhoAmI,               0, rplReadWhoAmI.Length);
                        replySize = devicePort.Read(rplReadMajorHardwareVersion, 0, rplReadMajorHardwareVersion.Length);
                        replySize = devicePort.Read(rplReadMinorHardwareVersion, 0, rplReadMinorHardwareVersion.Length);
                        replySize = devicePort.Read(rplReadMajorFirmwareVersion, 0, rplReadMajorFirmwareVersion.Length);
                        replySize = devicePort.Read(rplReadMinorFirmwareVersion, 0, rplReadMinorFirmwareVersion.Length);
                        replySize = devicePort.Read(rplReadTimestampSeconds,     0, rplReadTimestampSeconds.Length);

                        /* Parse mandatory registers */
                        int whoAmI = BitConverter.ToUInt16(rplReadWhoAmI, 11);
                        float HwVersion = rplReadMajorHardwareVersion[11] + rplReadMinorHardwareVersion[11] / (float)1000;
                        float FwVersion = rplReadMajorFirmwareVersion[11] + rplReadMinorFirmwareVersion[11] / (float)1000;
                        UInt32 timestamp = BitConverter.ToUInt32(rplReadTimestampSeconds, 11);

                        /************************************************************************/
                        /* Read Device Name register                                            */
                        /************************************************************************/
                        string deviceName = "";
                        bool nameAvailable = false;

                        devicePort.Write(cmdReadDeviceName.MessageBytes, 0, cmdReadWhoAmI.MessageBytes.Length);
                        
                        /* Wait until the first byte is available or until timeout is achieved */
                        timeout_ms = 500;
                        while ((devicePort.BytesToRead < 1) && timeout_ms > 0)
                        {
                            Thread.Sleep(1);
                            timeout_ms--;
                        }

                        /* Check if the Errormask is set on the reply's messageType  */
                        if (timeout_ms != 0)
                        {
                            const byte ErrorMask = 0x08;

                            replySize = devicePort.Read(rplReadDeviceName, 0, 1);
                            byte messageType = rplReadDeviceName[0];
                            

                            if ((messageType & ErrorMask) != ErrorMask)
                            {
                                /* Get the rest of the bytes */
                                timeout_ms = 500;
                                while ((devicePort.BytesToRead < rplReadDeviceName.Length - 1) && timeout_ms > 0)
                                {
                                    Thread.Sleep(1);
                                    timeout_ms--;
                                }

                                /* If the timeout wasn't achieved it means that the device name is availbale */
                                if (timeout_ms != 0)
                                {
                                    nameAvailable = true;

                                    replySize = devicePort.Read(rplReadDeviceName, 1, rplReadDeviceName.Length - 1);

                                    var deviceNameRegister = new byte[25];
                                    Array.Copy(rplReadDeviceName, 11, deviceNameRegister, 0, 25);                                                                        
                                    deviceName = System.Text.Encoding.Default.GetString(deviceNameRegister);
                                }
                            }
                            else
                            {
                                /* In case of error detected, flush the remaining bytes */
                                devicePort.DiscardInBuffer();
                            }
                        }

                        /************************************************************************/
                        /* Update Bonsai's Node name                                            */
                        /************************************************************************/
                        //string console = "Serial Harp device.";
                        //console += " WhoAmI: " + whoAmI;
                        //console += " Hw: " + rplReadMajorHardwareVersion[11] + "." + rplReadMinorHardwareVersion[11];
                        //console += " Fw: " + rplReadMajorFirmwareVersion[11] + "." + rplReadMinorFirmwareVersion[11];
                        //console += " Timestamp(s): " + timestamp;
                        //if (nameAvailable == true)
                            //console += " DeviceName: " + deviceName;
                        //Console.WriteLine(console);
                        Console.WriteLine("Serial Harp device.");
                        Console.WriteLine("WhoAmI: " + whoAmI);
                        Console.WriteLine("Hw: " + rplReadMajorHardwareVersion[11] + "." + rplReadMinorHardwareVersion[11]);
                        Console.WriteLine("Fw: " + rplReadMajorFirmwareVersion[11] + "." + rplReadMinorFirmwareVersion[11]);
                        Console.WriteLine("Timestamp(s): " + timestamp);
                        if (nameAvailable == true)
                            Console.WriteLine("UserDeviceName: " + deviceName);
                        Console.WriteLine("");

                        /* Return device name if available */
                        if ((nameAvailable == true) && (deviceName[0] != 0))
                        {
                            return deviceName;
                        }

                        /* Return from the device's list name */
                        if ((whoAmI >= 2048) && (whoAmI < 2064))
                            return ("MindReach");

                        switch (whoAmI)
                        {
                            case 1024:
                                return ("Poke");
                            case 1040:
                                return ("MultiPwm");
                            case 1056:
                                return ("Wear");
                            case 1072:
                                return ("12VoltsDrive");
                            case 1088:
                                return ("LedController");
                            case 1104:
                                return ("Synchronizer");
                            case 1121:
                                return ("SimpleAnalogGenerator");
                            case 1136:
                                return ("Archimedes");
                            case 1152:
                                return ("ClockSynchronizer");
                            case 1168:
                                return ("Camera");
                            case 1184:
                                return ("PyControl");
                            case 1200:
                                return ("FlyPad");
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
                
                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState, ledState, visualIndicators, heartbeat, replies, DumpRegisters);
                transport.Write(new HarpMessage(cmdWriteOpCtrl));

                var cleanup = Disposable.Create(() =>
                {
                    //Console.WriteLine("!");
                    cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState.Standby, ledState, visualIndicators, heartbeat, replies, false);
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

                var cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState, ledState, visualIndicators, heartbeat, replies, DumpRegisters);
                transport.Write(new HarpMessage(cmdWriteOpCtrl));

                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Subscribe(
                    transport.Write,
                    observer.OnError,
                    observer.OnCompleted);

                var cleanup = Disposable.Create(() =>
                {
                    //Console.WriteLine("!");
                    cmdWriteOpCtrl = CreateWriteOpCtrlCmd(DeviceState.Standby, ledState, visualIndicators, heartbeat, replies, false);
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
