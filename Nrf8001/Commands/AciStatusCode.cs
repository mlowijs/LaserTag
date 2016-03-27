
namespace Nrf8001Driver.Commands
{
    public enum AciStatusCode : byte
    {
        Success = 0x00,
        TransactionContinue = 0x01,
        TransactionComplete = 0x02,

        BondRequired = 0x8D
    }
}
