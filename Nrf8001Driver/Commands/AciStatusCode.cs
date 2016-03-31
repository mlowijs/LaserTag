
namespace Nrf8001Driver.Commands
{
    public enum AciStatusCode : byte
    {
        Success = 0x00,
        TransactionContinue = 0x01,
        TransactionComplete = 0x02,
        Extended = 0x03,

        BondRequired = 0x8D,

        AdvertisingTimeout = 0x93,
        PeerSmpError = 0x94
    }
}
