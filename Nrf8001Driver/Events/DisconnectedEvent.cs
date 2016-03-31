using Nrf8001Driver.Commands;

namespace Nrf8001Driver.Events
{
    public class DisconnectedEvent : AciEvent
    {
        public AciStatusCode StatusCode
        {
            get { return (AciStatusCode)Content[1]; }
        }

        public DisconnectedEvent(byte[] content)
            : base(content)
        {
        }
    }
}
