using LaserTag.Gun.Extensions;
using LaserTag.Gun.Laser;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LaserTag.Gun.Controllers
{
    public class IOController
    {
        private InputPort _triggerButton;

        private LaserDriver _laserDriver;

        public GameController GameController { get; set; }

        private bool _redDotSightEnabled;
        public bool RedDotSightEnabled
        {
            get { return _redDotSightEnabled; }
            set
            {
                _redDotSightEnabled = value;

                // TODO: RDS power
            }
        }

        public byte GunId { get; set; }

        public IOController()
        {
            _triggerButton = new InputPort(Pins.GPIO_PIN_D4, true, ResistorModes.PullUp);

            _laserDriver = new LaserDriver(SerialPorts.COM1);
            _laserDriver.PacketReceived += OnLaserPacketReceived;
        }


        public void Process()
        {
            GameController.FiringTimer.IsStarted = _triggerButton.IsPressed();
        }


        public void FireLaser()
        {
            _laserDriver.SendPacket(GunId);
        }


        private void OnLaserPacketReceived(LaserPacket packet)
        {
            GameController.HitByLaser(packet.SenderId);
        }
    }
}
