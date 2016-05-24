using Microsoft.SPOT;

namespace LaserTag.Gun.Controllers
{
    public class GameController
    {
        private const int FiringInterval = 300;

        private int _ammoPerClip = 20;
        private int _damagePerHit = 10;

        private int _initialClips = 3;
        private int _initialHealth = 100;

        private int _ammo;
        private int _clips;
        private int _health;

        public IBluetoothController BluetoothController { get; private set; }
        public IOController IOController { get; private set; }

        public Timer FiringTimer { get; private set; }

        public GameController(IBluetoothController btController, IOController ioController)
        {
            BluetoothController = btController;
            IOController = ioController;

            FiringTimer = new Timer(TryFireLaser, 0, FiringInterval);
        }


        public void TryReloadGun()
        {
            if (_clips > 0)
            {
                _clips--;
                _ammo = _ammoPerClip;

                BluetoothController.NotifyGun(_ammo, _clips);
            }
        }

        public void HitByLaser(byte shooterId)
        {
            if (_health <= 0)
                return;

            _health -= _damagePerHit;

            BluetoothController.NotifyPlayer(_health, shooterId);
        }

        public void Respawn()
        {
            // TODO: Add a timeout after dying

            _ammo = _ammoPerClip;
            _clips = _initialClips - 1;
            _health = _initialHealth;

            BluetoothController.NotifyGun(_ammo, _clips);
            BluetoothController.NotifyPlayer(_health, 0);
        }


        private void TryFireLaser()
        {
            if (_ammo > 0 && _health > 0)
            {
                _ammo--;

                IOController.FireLaser();
                BluetoothController.NotifyGun(_ammo, _clips);
            }
        }
    }
}
