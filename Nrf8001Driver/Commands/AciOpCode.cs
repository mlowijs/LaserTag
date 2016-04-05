
namespace Nrf8001Driver.Commands
{
    public enum AciOpCode : byte
    {
        Test = 0x01,
        Echo = 0x02,
        Sleep = 0x04,
        Wakeup = 0x05,
        Setup = 0x06,
        ReadDynamicData = 0x07,
        WriteDynamicData = 0x08,
        Connect = 0x0F,
        Bond = 0x10,
        OpenRemotePipe = 0x14,
        SendData = 0x15,
    }
}
