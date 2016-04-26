using LaserTag.Gun.Controllers;

namespace LaserTag.Gun.Controllers
{
    public interface IBluetoothController
    {
        GameController GameController { get; set; }
        IOController IOController { get; set; }

        void Process();
        void NotifyGun(int ammo, int clips);
        void NotifyPlayer(int health, byte shooterId);
    }
}
