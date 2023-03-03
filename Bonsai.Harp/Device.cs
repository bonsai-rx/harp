﻿using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Reactive.Concurrency;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an observable source of messages from the Harp device connected at the specified serial port.
    /// </summary>
    [XmlType(Namespace = Constants.XmlNamespace)]
    [Editor("Bonsai.Harp.Design.DeviceConfigurationEditor, Bonsai.Harp.Design", typeof(ComponentEditor))]
    [Description("Produces a sequence of messages from the Harp device connected at the specified serial port.")]
    public class Device : Source<HarpMessage>, INamedElement
    {
        string name;
        string portName;
        readonly int deviceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class
        /// accepting connections only from Harp devices with the specified
        /// <paramref name="whoAmI"/> identifier.
        /// </summary>
        /// <param name="whoAmI">The device identifier to match against serial connections.</param>
        public Device(int whoAmI)
        {
            deviceId = whoAmI;
            portName = "COMx";
            DeviceState = DeviceState.Active;
            LedState = LedState.On;
            VisualIndicators = LedState.On;
            DumpRegisters = true;
            Heartbeat = EnableType.Disable;
            CommandReplies = EnableType.Enable;
        }

        /// <summary>
        /// Gets or sets the state of the device at run time.
        /// </summary>
        [Description("Specifies the state of the device at run time.")]
        public DeviceState DeviceState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the device should send the content of all registers during initialization.
        /// </summary>
        [Description("Specifies whether the device should send the content of all registers during initialization.")]
        public bool DumpRegisters { get; set; }

        /// <summary>
        /// Gets or sets the state of the device LED.
        /// </summary>
        [Description("Specifies the state of the device LED.")]
        public LedState LedState { get; set; }

        /// <summary>
        /// Gets or sets the state of all the visual indicators in the device.
        /// </summary>
        [Description("Specifies the state of all the visual indicators in the device.")]
        public LedState VisualIndicators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Device sends the Timestamp event each second.
        /// </summary>
        [Description("Specifies if the Device sends the Timestamp event each second.")]
        public EnableType Heartbeat { get; set; }

        [Description("Specifies if the Device replies to commands.")]
        EnableType CommandReplies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether error messages parsed during acquisition should be ignored or raise an exception.
        /// </summary>
        [Description("Specifies whether error messages parsed during acquisition should be ignored or raise an error.")]
        public bool IgnoreErrors { get; set; }

        /// <summary>
        /// Gets or sets the name of the serial port used to communicate with the Harp device.
        /// </summary>
        [TypeConverter(typeof(PortNameConverter))]
        [Description("The name of the serial port used to communicate with the Harp device.")]
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                GetDeviceName(deviceId, portName, LedState, VisualIndicators, Heartbeat).Subscribe(deviceName => name = deviceName);
            }
        }

        static IObservable<string> GetDeviceName(int deviceId, string portName, LedState ledState, LedState visualIndicators, EnableType heartbeat)
        {
            return Observable.Create<string>(observer =>
            {
                var transport = default(SerialTransport);
                var writeOpCtrl = HarpCommand.OperationControl(DeviceState.Standby, ledState, visualIndicators, heartbeat, EnableType.Enable, false);
                var cmdReadWhoAmI = HarpCommand.ReadUInt16(DeviceRegisters.WhoAmI);
                var cmdReadMajorHardwareVersion = HarpCommand.ReadByte(DeviceRegisters.HardwareVersionHigh);
                var cmdReadMinorHardwareVersion = HarpCommand.ReadByte(DeviceRegisters.HardwareVersionLow);
                var cmdReadMajorFirmwareVersion = HarpCommand.ReadByte(DeviceRegisters.FirmwareVersionHigh);
                var cmdReadMinorFirmwareVersion = HarpCommand.ReadByte(DeviceRegisters.FirmwareVersionLow);
                var cmdReadTimestampSeconds = HarpCommand.ReadUInt32(DeviceRegisters.TimestampSecond);
                var cmdReadDeviceName = HarpCommand.ReadByte(DeviceRegisters.DeviceName);
                var cmdReadSerialNumber = HarpCommand.ReadUInt16(DeviceRegisters.SerialNumber);

                var whoAmI = 0;
                var timestamp = 0u;
                var hardwareVersionHigh = 0;
                var hardwareVersionLow = 0;
                var firmwareVersionHigh = 0;
                var firmwareVersionLow = 0;
                var serialNumber = default(ushort?);
                var messageObserver = Observer.Create<HarpMessage>(
                    message =>
                    {
                        switch (message.Address)
                        {
                            case DeviceRegisters.OperationControl:
                                transport.Write(cmdReadWhoAmI);
                                transport.Write(cmdReadMajorHardwareVersion);
                                transport.Write(cmdReadMinorHardwareVersion);
                                transport.Write(cmdReadMajorFirmwareVersion);
                                transport.Write(cmdReadMinorFirmwareVersion);
                                transport.Write(cmdReadTimestampSeconds);
                                transport.Write(cmdReadSerialNumber);
                                transport.Write(cmdReadDeviceName);
                                break;
                            case DeviceRegisters.WhoAmI: whoAmI = message.GetPayloadUInt16(); break;
                            case DeviceRegisters.HardwareVersionHigh: hardwareVersionHigh = message.GetPayloadByte(); break;
                            case DeviceRegisters.HardwareVersionLow: hardwareVersionLow = message.GetPayloadByte(); break;
                            case DeviceRegisters.FirmwareVersionHigh: firmwareVersionHigh = message.GetPayloadByte(); break;
                            case DeviceRegisters.FirmwareVersionLow: firmwareVersionLow = message.GetPayloadByte(); break;
                            case DeviceRegisters.TimestampSecond: timestamp = message.GetPayloadUInt32(); break;
                            case DeviceRegisters.SerialNumber: if (!message.Error) serialNumber = message.GetPayloadUInt16(); break;
                            case DeviceRegisters.DeviceName:
                                var deviceName = nameof(Device);
                                if (!message.Error)
                                {
                                    var namePayload = message.GetPayload();
                                    deviceName = Encoding.ASCII.GetString(namePayload.Array, namePayload.Offset, namePayload.Count);
                                }
                                Console.WriteLine("Serial Harp device.");
                                if (deviceId > 0 && deviceId != whoAmI) Console.WriteLine("WARNING: Unexpected device identifier!");
                                if (!serialNumber.HasValue) Console.WriteLine($"WhoAmI: {whoAmI}");
                                else Console.WriteLine($"WhoAmI: {whoAmI}-{serialNumber:x4}");
                                Console.WriteLine($"Hw: {hardwareVersionHigh}.{hardwareVersionLow}");
                                Console.WriteLine($"Fw: {firmwareVersionHigh}.{firmwareVersionLow}");
                                Console.WriteLine($"Timestamp (s): {timestamp}");
                                Console.WriteLine($"DeviceName: {deviceName}");
                                Console.WriteLine();
                                observer.OnNext(deviceName);
                                observer.OnCompleted();
                                break;
                            default:
                                break;
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted);
                transport = new SerialTransport(portName, messageObserver);
                transport.IgnoreErrors = true;
                transport.Open();

                transport.Write(writeOpCtrl);
                return transport;
            }).Timeout(TimeSpan.FromMilliseconds(500))
              .OnErrorResumeNext(Observable.Return(nameof(Device)))
              .SubscribeOn(Scheduler.Default)
              .FirstAsync();
        }

        /// <summary>
        /// Connects to the specified serial port and returns an observable sequence of Harp messages
        /// coming from the device.
        /// </summary>
        /// <returns>The observable sequence of Harp messages produced by the device.</returns>
        public override IObservable<HarpMessage> Generate()
        {
            return Observable.Create<HarpMessage>(observer =>
            {
                var transport = CreateTransport(observer);
                var cleanup = Disposable.Create(() =>
                {
                    var writeOpCtrl = HarpCommand.OperationControl(DeviceState.Standby, LedState, VisualIndicators, Heartbeat, CommandReplies, false);
                    transport.Write(writeOpCtrl);
                });

                return new CompositeDisposable(
                    cleanup,
                    transport);
            });
        }

        /// <summary>
        /// Connects to the specified serial port and sends the observable sequence of Harp messages.
        /// The return value is an observable sequence of Harp messages coming from the device.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages to send to the device.</param>
        /// <returns>The observable sequence of Harp messages produced by the device.</returns>
        public IObservable<HarpMessage> Generate(IObservable<HarpMessage> source)
        {
            return Observable.Create<HarpMessage>(observer =>
            {
                var transport = CreateTransport(observer);
                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Subscribe(
                    transport.Write,
                    observer.OnError,
                    observer.OnCompleted);

                var cleanup = Disposable.Create(() =>
                {
                    var writeOpCtrl = HarpCommand.OperationControl(DeviceState.Standby, LedState, VisualIndicators, Heartbeat, CommandReplies, false);
                    transport.Write(writeOpCtrl);
                });

                return new CompositeDisposable(
                    cleanup,
                    sourceDisposable,
                    transport);
            });
        }

        string INamedElement.Name => !string.IsNullOrEmpty(name) ? name : nameof(Device);

        SerialTransport CreateTransport(IObserver<HarpMessage> observer)
        {
            var portName = PortName;
            var transport = new SerialTransport(portName, observer);
            transport.IgnoreErrors = IgnoreErrors;

            if (deviceId > 0)
            {
                transport.SetObserver(Observer.Create<HarpMessage>(
                    message =>
                    {
                        if (message.Address != DeviceRegisters.WhoAmI)
                        {
                            Console.Error.WriteLine("Unexpected Harp data frame before identifier: {0}.", message);
                            return;
                        }

                        var whoAmI = message.GetPayloadUInt16();
                        if (whoAmI != deviceId)
                        {
                            var errorMessage = string.Format(
                                "The device ID {1} on {0} was unexpected. Check whether the correct device is connected to the specified serial port.",
                                portName, whoAmI);
                            observer.OnError(new HarpException(errorMessage));
                            return;
                        }

                        transport.SetObserver(observer);
                    },
                    observer.OnError,
                    observer.OnCompleted));
                transport.Open();

                var cmdReadWhoAmI = HarpCommand.ReadUInt16(DeviceRegisters.WhoAmI);
                transport.Write(cmdReadWhoAmI);
            }
            else transport.Open();

            var writeOpCtrl = HarpCommand.OperationControl(DeviceState, LedState, VisualIndicators, Heartbeat, CommandReplies, DumpRegisters);
            transport.Write(writeOpCtrl);
            return transport;
        }
    }
}
