namespace Microchip25xx080Driver
{
    public enum Instruction : byte
    {
        WriteStatusRegister = 0x01,
        Write = 0x02,
        Read = 0x03,
        ReadStatusRegister = 0x05,
        WriteDisable = 0x04,
        WriteEnable = 0x06,
    }
}
