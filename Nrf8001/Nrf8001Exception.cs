using System;

namespace Nrf8001Driver
{
    public class Nrf8001Exception : Exception
    {
        public Nrf8001Exception(string message)
            : base(message)
        {
        }
    }
}
