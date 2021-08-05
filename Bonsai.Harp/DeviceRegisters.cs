namespace Bonsai.Harp
{
    /// <summary>
    /// Provides address and payload types for common registers and operation control
    /// of all Harp devices.
    /// </summary>
    public static class DeviceRegisters
    {
        /// <summary>
        /// The address of the WHO_AM_I register used to verify the identity class of
        /// the device. This field is read-only.
        /// </summary>
        public const int WhoAmI = 0;

        /// <summary>
        /// The address of the HW_VERSION_H register containing the major hardware
        /// version of the device. This field is read-only.
        /// </summary>
        public const int HardwareVersionHigh = 1;

        /// <summary>
        /// The address of the HW_VERSION_L register containing the minor hardware
        /// version of the device. This field is read-only.
        /// </summary>
        public const int HardwareVersionLow = 2;

        /// <summary>
        /// The address of the ASSEMBLY_VERSION register containing the version of
        /// the assembled components in the device. This field is read-only.
        /// </summary>
        public const int AssemblyVersion = 3;

        /// <summary>
        /// The address of the HARP_VERSION_H register containing the major version
        /// of the Harp core used by the device. This field is read-only.
        /// </summary>
        public const int CoreVersionHigh = 4;

        /// <summary>
        /// The address of the HARP_VERSION_L register containing the minor version
        /// of the Harp core used by the device. This field is read-only.
        /// </summary>
        public const int CoreVersionLow = 5;

        /// <summary>
        /// The address of the FW_VERSION_H register containing the major firmware
        /// version installed in the device. This field is read-only.
        /// </summary>
        public const int FirmwareVersionHigh = 6;

        /// <summary>
        /// The address of the FW_VERSION_L register containing the minor firmware
        /// version installed in the device. This field is read-only.
        /// </summary>
        public const int FirmwareVersionLow = 7;

        /// <summary>
        /// The address of the TIMESTAMP_SECOND register containing the integral part of
        /// the system timestamp, in seconds. This field is read-only.
        /// </summary>
        public const int TimestampSecond = 8;

        /// <summary>
        /// The address of the TIMESTAMP_MICRO register containing the fractional part of
        /// the system timestamp, in microseconds. This field is read-only.
        /// </summary>
        public const int TimestampMicrosecond = 9;

        /// <summary>
        /// The address of the OPERATION_CTRL register containing the configuration of the
        /// operation mode of the device. This field is read-only.
        /// </summary>
        public const int OperationControl = 10;

        /// <summary>
        /// The address of the RESET_DEV register used to reset the device and save
        /// non-volatile registers. This field is read-only.
        /// </summary>
        public const int ResetDevice = 11;

        /// <summary>
        /// The address of the DEVICE_NAME register containing the user configurable name
        /// for the device. This field is read-only.
        /// </summary>
        public const int DeviceName = 12;

        /// <summary>
        /// The address of the SERIAL_NUMBER register containing the unique serial number
        /// of the device. This field is read-only.
        /// </summary>
        public const int SerialNumber = 13;

        /// <summary>
        /// The payload type of the WHO_AM_I register. This field is read-only.
        /// </summary>
        public const PayloadType WhoAmIPayload = PayloadType.U16;

        /// <summary>
        /// The payload type of the HW_VERSION_H register. This field is read-only.
        /// </summary>
        public const PayloadType HardwareVersionHighPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the HW_VERSION_L register. This field is read-only.
        /// </summary>
        public const PayloadType HardwareVersionLowPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the ASSEMBLY_VERSION register. This field is read-only.
        /// </summary>
        public const PayloadType AssemblyVersionPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the HARP_VERSION_H register. This field is read-only.
        /// </summary>
        public const PayloadType CoreVersionHighPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the HARP_VERSION_L register. This field is read-only.
        /// </summary>
        public const PayloadType CoreVersionLowPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the FW_VERSION_H register. This field is read-only.
        /// </summary>
        public const PayloadType FirmwareVersionHighPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the FW_VERSION_L register. This field is read-only.
        /// </summary>
        public const PayloadType FirmwareVersionLowPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the TIMESTAMP_SECOND register. This field is read-only.
        /// </summary>
        public const PayloadType TimestampSecondPayload = PayloadType.U32;

        /// <summary>
        /// The payload type of the TIMESTAMP_MICRO register. This field is read-only.
        /// </summary>
        public const PayloadType TimestampMicrosecondPayload = PayloadType.U16;

        /// <summary>
        /// The payload type of the RESET_DEV register. This field is read-only.
        /// </summary>
        public const PayloadType ResetDevicePayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the OPERATION_CTRL register. This field is read-only.
        /// </summary>
        public const PayloadType OperationControlPayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the DEVICE_NAME register. This field is read-only.
        /// </summary>
        public const PayloadType DeviceNamePayload = PayloadType.U8;

        /// <summary>
        /// The payload type of the SERIAL_NUMBER register. This field is read-only.
        /// </summary>
        public const PayloadType SerialNumberPayload = PayloadType.U16;
    }
}
