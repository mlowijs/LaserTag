namespace LaserTag.Controllers
{
    public class GameController
    {
        public int Health { get; private set; }
        public int Ammo { get; private set; }
        public int Clips { get; private set; }

        private int _initialHealth;
        private int _damagePerShot;
        private int _ammoPerClip;


        public void InitializeGame()
        {
            // Do all initialization here
        }

        public void RegisterHit()
        {
            Health -= _damagePerShot;

            if (Health <= 0)
            {
                // you died
            }
        }

        public void Respawn()
        {
            Health = _initialHealth;
        }


        public void ReloadAmmo()
        {
            if (Clips > 0)
            {
                Clips--;

                Ammo = _ammoPerClip;
            }
        }

        public void FireShot()
        {
            if (Ammo > 0)
                Ammo--;
        }
    }
}
