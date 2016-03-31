using Nrf8001Driver.Commands;

namespace Nrf8001Driver.Events
{
    public class BondStatusEvent : AciEvent
    {
        public BondStatusCode StatusCode
        {
            get { return (BondStatusCode)Content[1]; }
        }

        public BondStatusEvent(byte[] content)
            : base(content)
        {
        }
    }
}
