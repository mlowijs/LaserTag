using LaserTag.Laser;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LaserTag.Controllers
{
    public class GunController
    {
        private const int ShotRate = 200;

        private InputPort _triggerButton;
        private LaserDriver _laserDriver;
        private Timer _triggerTimer;

        public GunController(byte gunId)
        {
            _triggerButton = new InputPort(Pins.GPIO_PIN_D4, true, ResistorModes.PullUp);
            _triggerTimer = new Timer(_ => _laserDriver.SendPacket(), 0, ShotRate);

            _laserDriver = new LaserDriver(SerialPorts.COM1, gunId);
            _laserDriver.PacketReceived += OnLaserPacketReceived;
        }


        public void Process()
        {
            _triggerTimer.IsEnabled = _triggerButton.IsPressed();
        }


        private void OnLaserPacketReceived(LaserPacket packet)
        {
            //_nrf.SendData(GunHitPipeId, 0x01);

            Debug.Print("[" + packet.SequenceNumber + "] HIT by gun ID " + packet.SenderId);
        }
    }
}
