namespace Bonsai.Harp
{
    internal static class Registers
    {
        public const byte HardwareVersionHigh = 1;
        public const PayloadType HardwareVersionHighPayload = PayloadType.U8;

        public const byte HardwareVersionLow = 2;
        public const PayloadType HardwareVersionLowPayload = PayloadType.U8;

        public const byte TimestampSecond = 8;
        public const PayloadType TimestampSecondPayload = PayloadType.U32;

        public const byte DeviceName = 12;
        public const PayloadType DeviceNamePayload = PayloadType.U8;
    }
}
