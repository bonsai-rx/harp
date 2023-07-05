using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Threading;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an observable source of messages from the Harp device connected at the specified serial port.
    /// </summary>
    [XmlType(Namespace = Constants.XmlNamespace)]
    [Editor("Bonsai.Harp.Design.DeviceConfigurationEditor, Bonsai.Harp.Design", typeof(ComponentEditor))]
    [Description("Produces a sequence of messages from the Harp device connected at the specified serial port.")]
    public partial class Device : Source<HarpMessage>, INamedElement
    {
        string name;
        string portName;
        readonly int deviceId;
        readonly FirmwareMetadata deviceFirmware;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class
        /// accepting connections only from Harp devices with the specified identifier.
        /// </summary>
        /// <param name="whoAmI">The device identifier to match against serial connections.</param>
        public Device(int whoAmI)
            : this(whoAmI, firmware: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class
        /// accepting connections only from Harp devices with the specified
        /// identifier and firmware version.
        /// </summary>
        /// <param name="whoAmI">The device identifier to match against serial connections.</param>
        /// <param name="firmware">Provides information about the expected device firmware version.</param>
        public Device(int whoAmI, FirmwareMetadata firmware)
        {
            deviceId = whoAmI;
            deviceFirmware = firmware;
            if (deviceFirmware != null && deviceId == 0)
            {
                throw new ArgumentException(
                    "A valid device identifier must be specified when firmware metadata is provided.",
                    nameof(whoAmI));
            }

            portName = "COMx";
            OperationMode = OperationMode.Active;
            OperationLed = LedState.On;
            VisualIndicators = LedState.On;
            DumpRegisters = true;
            Heartbeat = EnableFlag.Disabled;
            MuteReplies = false;
        }

        /// <summary>
        /// Gets or sets a value specifying the operation mode of the device at initialization.
        /// </summary>
        [Description("Specifies the operation mode of the device at initialization.")]
        public OperationMode OperationMode { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the state of the LED reporting device operation.
        /// </summary>
        [Description("Specifies the state of the LED reporting device operation.")]
        public LedState OperationLed { get; set; }

#pragma warning disable CS0612 // Type or member is obsolete
        /// <summary>
        /// Gets or sets the state of the device at run time.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("Specifies the state of the device at run time.")]
        public DeviceState DeviceState
        {
            get { return OperationMode == OperationMode.Active ? DeviceState.Active : DeviceState.Standby; }
            set { OperationMode = value == DeviceState.Active ? OperationMode.Active : OperationMode.Standby; }
        }

        /// <summary>
        /// Gets or sets the state of the device LED.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("Specifies the state of the device LED.")]
        public LedState LedState
        {
            get { return OperationLed; }
            set { OperationLed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="DeviceState"/> property should be serialized.
        /// </summary>
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDeviceState() => false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="LedState"/> property should be serialized.
        /// </summary>
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeLedState() => false;


        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void set_Heartbeat(EnableType value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Heartbeat = value == EnableType.Enable ? EnableFlag.Enabled : EnableFlag.Disabled;
        }
#pragma warning restore CS0612 // Type or member is obsolete

        /// <summary>
        /// Gets or sets a value indicating whether the device should send the content of all registers during initialization.
        /// </summary>
        [Description("Specifies whether the device should send the content of all registers during initialization.")]
        public bool DumpRegisters { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the state of all the visual indicators in the device.
        /// </summary>
        [Description("Specifies the state of all the visual indicators in the device.")]
        public LedState VisualIndicators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Device sends the Timestamp event each second.
        /// </summary>
        [Description("Specifies if the Device sends the Timestamp event each second.")]
        public EnableFlag Heartbeat { get; set; }

        [Description("Specifies if the Device replies to commands.")]
        bool MuteReplies { get; set; }

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
                if (deviceId == 0)
                {
                    GetDeviceName(portName, LedState, VisualIndicators, Heartbeat).Subscribe(deviceName => name = deviceName);
                }
            }
        }

        static IObservable<string> GetDeviceName(string portName, LedState ledState, LedState visualIndicators, EnableFlag heartbeat)
        {
            return Observable.Create<string>(observer =>
            {
                var transport = default(SerialTransport);
                var writeOpCtrl = OperationControl.FromPayload(MessageType.Write, new OperationControlPayload(
                    OperationMode.Standby,
                    dumpRegisters: false,
                    muteReplies: false,
                    ledState,
                    visualIndicators,
                    heartbeat));
                var cmdReadWhoAmI = HarpCommand.ReadUInt16(WhoAmI.Address);
                var cmdReadMajorHardwareVersion = HarpCommand.ReadByte(HardwareVersionHigh.Address);
                var cmdReadMinorHardwareVersion = HarpCommand.ReadByte(HardwareVersionLow.Address);
                var cmdReadMajorFirmwareVersion = HarpCommand.ReadByte(FirmwareVersionHigh.Address);
                var cmdReadMinorFirmwareVersion = HarpCommand.ReadByte(FirmwareVersionLow.Address);
                var cmdReadTimestampSeconds = HarpCommand.ReadUInt32(TimestampSeconds.Address);
                var cmdReadDeviceName = HarpCommand.ReadByte(DeviceName.Address);
                var cmdReadSerialNumber = HarpCommand.ReadUInt16(SerialNumber.Address);

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
                            case OperationControl.Address:
                                transport.Write(cmdReadWhoAmI);
                                transport.Write(cmdReadMajorHardwareVersion);
                                transport.Write(cmdReadMinorHardwareVersion);
                                transport.Write(cmdReadMajorFirmwareVersion);
                                transport.Write(cmdReadMinorFirmwareVersion);
                                transport.Write(cmdReadTimestampSeconds);
                                transport.Write(cmdReadSerialNumber);
                                transport.Write(cmdReadDeviceName);
                                break;
                            case WhoAmI.Address: whoAmI = WhoAmI.GetPayload(message); break;
                            case HardwareVersionHigh.Address: hardwareVersionHigh = HardwareVersionHigh.GetPayload(message); break;
                            case HardwareVersionLow.Address: hardwareVersionLow = HardwareVersionLow.GetPayload(message); break;
                            case FirmwareVersionHigh.Address: firmwareVersionHigh = FirmwareVersionHigh.GetPayload(message); break;
                            case FirmwareVersionLow.Address: firmwareVersionLow = FirmwareVersionLow.GetPayload(message); break;
                            case TimestampSeconds.Address: timestamp = TimestampSeconds.GetPayload(message); break;
                            case SerialNumber.Address: if (!message.Error) serialNumber = SerialNumber.GetPayload(message); break;
                            case DeviceName.Address:
                                var deviceName = nameof(Device);
                                if (!message.Error) deviceName = DeviceName.GetPayload(message);
                                Console.WriteLine("Serial Harp device.");
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
            return Observable.Create<HarpMessage>(async (observer, cancellationToken) =>
            {
                var transport = await CreateTransportAsync(observer, cancellationToken);
                return Disposable.Create(() =>
                {
                    var writeOpCtrl = OperationControl.FromPayload(MessageType.Write, new OperationControlPayload(
                        OperationMode.Standby,
                        dumpRegisters: false,
                        MuteReplies,
                        VisualIndicators,
                        OperationLed,
                        Heartbeat));
                    transport.Write(writeOpCtrl);
                    transport.Close();
                });
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
            return Observable.Create<HarpMessage>(async (observer, cancellationToken) =>
            {
                var transport = await CreateTransportAsync(observer, cancellationToken);
                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Subscribe(
                    transport.Write,
                    observer.OnError,
                    observer.OnCompleted);

                return Disposable.Create(() =>
                {
                    var writeOpCtrl = OperationControl.FromPayload(MessageType.Write, new OperationControlPayload(
                        OperationMode.Standby,
                        dumpRegisters: false,
                        MuteReplies,
                        VisualIndicators,
                        OperationLed,
                        Heartbeat));
                    transport.Write(writeOpCtrl);
                    sourceDisposable.Dispose();
                    transport.Close();
                });
            });
        }

        string INamedElement.Name => !string.IsNullOrEmpty(name) ? name : default;

        async Task<SerialTransport> CreateTransportAsync(IObserver<HarpMessage> observer, CancellationToken cancellationToken)
        {
            var portName = PortName;
            SerialTransport transport;
            using (var device = new AsyncDevice(portName, leaveOpen: true))
            {
                try
                {
                    var whoAmI = await device.ReadWhoAmIAsync(cancellationToken);
                    if (deviceId > 0 && whoAmI != deviceId)
                    {
                        throw new HarpException(string.Format(
                            "The device ID {1} on {0} was unexpected. Check whether the correct device is connected to the specified serial port.",
                            portName, whoAmI));
                    }

                    if (deviceFirmware != null)
                    {
                        var firmwareVersion = await device.ReadFirmwareVersionAsync(cancellationToken);
                        if (firmwareVersion != deviceFirmware.FirmwareVersion)
                        {
                            throw new HarpException(string.Format(
                                "The device firmware version was unexpected. Expected version {0} and device reported {1}.",
                                deviceFirmware.FirmwareVersion, firmwareVersion));
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    device.Transport.Close();
                    throw;
                }

                transport = device.Transport;
                transport.IgnoreErrors = IgnoreErrors;
                transport.SetObserver(Observer.Create<HarpMessage>(
                    message =>
                    {
                        if (message.Address != OperationControl.Address)
                        {
                            Console.Error.WriteLine("Unexpected Harp data frame before operation control: {0}.", message);
                            return;
                        }

                        transport.SetObserver(observer);
                    },
                    observer.OnError,
                    observer.OnCompleted));
            }

            var writeOpCtrl = OperationControl.FromPayload(MessageType.Write, new OperationControlPayload(
                OperationMode,
                DumpRegisters,
                MuteReplies,
                VisualIndicators,
                OperationLed,
                Heartbeat));
            transport.Write(writeOpCtrl);
            return transport;
        }
    }
}
