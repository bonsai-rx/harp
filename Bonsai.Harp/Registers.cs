namespace Bonsai.Harp
{
    internal static class Registers
    {
        public const int WhoAmI = 0;
        public const int HardwareVersionHigh = 1;
        public const int HardwareVersionLow = 2;
        public const int FirmwareVersionHigh = 6;
        public const int FirmwareVersionLow = 7;
        public const int TimestampSecond = 8;
        public const int OperationControl = 10;
        public const int Reset = 11;
        public const int DeviceName = 12;
        public const int SerialNumber = 13;

        public const PayloadType WhoAmIPayload = PayloadType.U16;
        public const PayloadType HardwareVersionHighPayload = PayloadType.U8;
        public const PayloadType HardwareVersionLowPayload = PayloadType.U8;
        public const PayloadType FirmwareVersionHighPayload = PayloadType.U8;
        public const PayloadType FirmwareVersionLowPayload = PayloadType.U8;
        public const PayloadType TimestampSecondPayload = PayloadType.U32;
        public const PayloadType OperationControlPayload = PayloadType.U8;
        public const PayloadType DeviceNamePayload = PayloadType.U8;
        public const PayloadType SerialNumberPayload = PayloadType.U16;
    }
}
