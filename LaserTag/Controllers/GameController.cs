namespace LaserTag.Controllers
{
    public class GameController
    {
        private const int FiringInterval = 250;

        private int _ammoPerClip = 20;
        private int _damagePerHit = 10;
        private int _initialHealth = 100;

        private int _ammo;
        private int _clips;
        private int _health;

        public BluetoothController BluetoothController { get; private set; }
        public IOController IOController { get; private set; }

        public Timer FiringTimer { get; private set; }

        public GameController(BluetoothController btController, IOController ioController)
        {
            BluetoothController = btController;
            IOController = ioController;

            FiringTimer = new Timer(_ => TryFireLaser(), 0, FiringInterval);
        }


        public void TryReloadGun()
        {
            if (_clips > 0)
            {
                _clips--;
                _ammo = _ammoPerClip;

                BluetoothController.NotifyAmmo(_ammo);
                BluetoothController.NotifyClips(_clips);
            }
        }

        public void HitByLaser(byte shooterId)
        {
            _health -= _damagePerHit;

            if (_health <= 0)
                _health = _initialHealth; // TODO: respawn mechanismb

            BluetoothController.NotifyHealth(_health, shooterId);
        }


        private void TryFireLaser()
        {
            if (_ammo > 0)
            {
                _ammo--;

                IOController.FireLaser();
                BluetoothController.NotifyAmmo(_ammo);
            }
        }
    }
}
