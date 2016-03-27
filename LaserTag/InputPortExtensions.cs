using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LaserTag
{
    public static class InputPortExtensions
    {
        public static bool IsPressed(this InputPort inputPort)
        {
            var read = inputPort.Read();

            return (inputPort.Resistor == ResistorModes.PullUp && !read) || (inputPort.Resistor == ResistorModes.PullDown && read);
        }
    }
}
