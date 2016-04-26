using Microsoft.SPOT;

namespace LaserTag.Gun.Controllers
{
    public class DummyBluetoothController : IBluetoothController
    {
        public GameController GameController { get; set; }
        public IOController IOController { get; set; }

        public void NotifyGun(int ammo, int clips)
        {
        }

        public void NotifyPlayer(int health, byte shooterId)
        {
        }

        public void Process()
        {
        }
    }
}
