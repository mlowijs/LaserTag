namespace Nrf8001Driver.Events
{
    public class DataCreditEvent : AciEvent
    {
        public byte DataCreditsAvailable
        {
            get { return Content[1]; }
        }

        public DataCreditEvent(byte[] content)
            : base(content)
        {
        }
    }
}
