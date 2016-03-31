using Nrf8001Driver.Commands;

namespace Nrf8001Driver.Events
{
    public class CommandResponseEvent : AciEvent
    {
        public AciOpCode Command
        {
            get { return (AciOpCode)Content[1]; }
        }

        public AciStatusCode StatusCode
        {
            get { return (AciStatusCode)Content[2]; }
        }

        public CommandResponseEvent(byte[] content)
            : base(content)
        {
        }
    }
}
