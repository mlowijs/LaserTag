using LaserTag.Extensions;
using LaserTag.Laser;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LaserTag.Controllers
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

                Debug.Print("TODO: RDS power");
            }
        }

        public IOController(byte gunId)
        {
            _triggerButton = new InputPort(Pins.GPIO_PIN_D4, true, ResistorModes.PullUp);

            _laserDriver = new LaserDriver(SerialPorts.COM1, gunId);
            _laserDriver.PacketReceived += OnLaserPacketReceived;
        }


        public void Process()
        {
            GameController.FiringTimer.IsStarted = _triggerButton.IsPressed();
        }


        public void FireLaser()
        {
            _laserDriver.SendPacket();
        }


        private void OnLaserPacketReceived(LaserPacket packet)
        {
            Debug.Print("[" + packet.SequenceNumber + "] HIT by gun ID " + packet.SenderId);

            GameController.HitByLaser(packet.SenderId);
        }
    }
}