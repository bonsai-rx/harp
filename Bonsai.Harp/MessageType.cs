namespace Bonsai.Harp
{
    public enum MessageType : byte
    {
        Read = 0x01,
        Write = 0x02,
        Event = 0x03
    }
}
