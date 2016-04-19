using LaserTag.Gun.Extensions;
using LaserTag.Model;
using Microsoft.SPOT.Hardware;
using Nrf8001Driver;
using Nrf8001Driver.Commands;
using Nrf8001Driver.Events;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Text;

namespace LaserTag.Gun.Controllers
{
    public class BluetoothController
    {
        #region nRF8001 Setup Data
        private readonly byte[][] Nrf8001SetupData = new byte[][]
        {
            new byte[] {0x00,0x00,0x03,0x02,0x42,0x07,},
            new byte[] {0x10,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x04,0x00,0x04,0x01,0x01,0x00,0x00,0x06,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0x10,0x1c,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x10,0x00,0x00,0x00,0x10,0x03,0x90,0x01,0xff,},
            new byte[] {0x10,0x38,0xff,0xff,0x02,0x58,0x00,0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0x10,0x54,0x01,0x00,},
            new byte[] {0x20,0x00,0x04,0x04,0x02,0x02,0x00,0x01,0x28,0x00,0x01,0x00,0x18,0x04,0x04,0x05,0x05,0x00,0x02,0x28,0x03,0x01,0x0e,0x03,0x00,0x00,0x2a,0x04,0x34,0x09,},
            new byte[] {0x20,0x1c,0x09,0x00,0x03,0x2a,0x00,0x01,0x4c,0x54,0x20,0x47,0x75,0x6e,0x20,0x30,0x30,0x04,0x04,0x05,0x05,0x00,0x04,0x28,0x03,0x01,0x02,0x05,0x00,0x01,},
            new byte[] {0x20,0x38,0x2a,0x06,0x04,0x03,0x02,0x00,0x05,0x2a,0x01,0x01,0xc0,0x03,0x04,0x04,0x05,0x05,0x00,0x06,0x28,0x03,0x01,0x02,0x07,0x00,0x04,0x2a,0x06,0x04,},
            new byte[] {0x20,0x54,0x09,0x08,0x00,0x07,0x2a,0x04,0x01,0x06,0x00,0x06,0x00,0x00,0x00,0xff,0xff,0x04,0x04,0x02,0x02,0x00,0x08,0x28,0x00,0x01,0x01,0x18,0x04,0x04,},
            new byte[] {0x20,0x70,0x10,0x10,0x00,0x09,0x28,0x00,0x01,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x00,0x20,0xfa,0xf0,0x04,0x04,0x13,0x13,0x00,},
            new byte[] {0x20,0x8c,0x0a,0x28,0x03,0x01,0x06,0x0b,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x01,0x20,0xfa,0xf0,0x44,0x3c,0x02,0x01,0x00,},
            new byte[] {0x20,0xa8,0x0b,0x20,0x01,0x02,0x00,0x00,0x04,0x04,0x10,0x10,0x00,0x0c,0x28,0x00,0x01,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x00,},
            new byte[] {0x20,0xc4,0x10,0xfa,0xf0,0x04,0x04,0x13,0x13,0x00,0x0d,0x28,0x03,0x01,0x12,0x0e,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x01,},
            new byte[] {0x20,0xe0,0x10,0xfa,0xf0,0x16,0x0c,0x03,0x02,0x00,0x0e,0x10,0x01,0x02,0x00,0x00,0x46,0x34,0x03,0x02,0x00,0x0f,0x29,0x02,0x01,0x00,0x00,0x04,0x04,0x13,},
            new byte[] {0x20,0xfc,0x13,0x00,0x10,0x28,0x03,0x01,0x12,0x11,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x02,0x10,0xfa,0xf0,0x16,0x0c,0x03,},
            new byte[] {0x21,0x18,0x02,0x00,0x11,0x10,0x02,0x02,0x00,0x00,0x46,0x34,0x03,0x02,0x00,0x12,0x29,0x02,0x01,0x00,0x00,0x00,},
            new byte[] {0x40,0x00,0x2a,0x00,0x01,0x00,0x80,0x04,0x00,0x03,0x00,0x00,0x20,0x01,0x02,0x00,0x08,0x04,0x00,0x0b,0x00,0x00,0x10,0x01,0x02,0x00,0x02,0x04,0x00,0x0e,},
            new byte[] {0x40,0x1c,0x00,0x0f,0x10,0x02,0x02,0x00,0x02,0x04,0x00,0x11,0x00,0x12,},
            new byte[] {0x50,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x00,0x00,0xfa,0xf0,},
            new byte[] {0x60,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0xf0,0x00,0x03,0xd8,0xdb,},
        };
        #endregion

        private const byte BtLeBondTimeout = 15;
        private const byte BtLeBondInterval = 64;

        private const byte DeviceNamePipeId = 1;
        private const byte CommandPipeId = 2;
        private const byte GunStatPipeId = 3;
        private const byte PlayerStatPipeId = 4;

        private static readonly byte[] DeviceNameBytes = new byte[] { 0x4c, 0x54, 0x20, 0x47, 0x75, 0x6e, 0x20 }; // "LT Gun "

        private Nrf8001 _nrf;
        private OutputPort _onboardLed;
        private Timer _btTimer;

        public GameController GameController { get; set; }
        public IOController IOController { get; private set; }

        public BluetoothController(IOController ioController)
        {
            _onboardLed = new OutputPort(Pins.ONBOARD_LED, false);
            _btTimer = new Timer(() => _onboardLed.Write(!_onboardLed.Read()), 0, 750);

            IOController = ioController;

            _nrf = new Nrf8001(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D7, SPI_Devices.SPI1);
            _nrf.AciEventReceived += OnAciEventReceived;
            _nrf.DataReceived += OnDataReceived;

            _nrf.Setup(Nrf8001SetupData);

            var deviceName = Encoding.UTF8.GetBytes("LT Gun " + ioController.GunId.ToString().PadLeft(2, '0'));
            _nrf.SetLocalData(DeviceNamePipeId, deviceName);

            _nrf.AwaitBond(BtLeBondTimeout, BtLeBondInterval);
        }


        public void Process()
        {
            _nrf.ProcessEvents();
        }


        public void NotifyGun(int ammo, int clips)
        {
            if (_nrf.IsServicePipeOpen(GunStatPipeId))
                _nrf.SendData(GunStatPipeId, (byte)ammo, (byte)clips);
        }

        public void NotifyPlayer(int health, byte shooterId)
        {
            if (_nrf.IsServicePipeOpen(PlayerStatPipeId))
                _nrf.SendData(PlayerStatPipeId, (byte)health, shooterId);
        }


        private void HandleCommand(byte[] data)
        {
            var command = (Command)data[0];

            if (command == Command.RdsPower)
                IOController.RedDotSightEnabled = data[1] == 0x01;

            if (command == Command.Reload)
                GameController.TryReloadGun();

            if (command == Command.Respawn)
                GameController.Respawn();
        }


        private void OnAciEventReceived(AciEvent aciEvent)
        {
            switch (aciEvent.EventType)
            {
                case AciEventType.CommandResponse:
                    var evt = (CommandResponseEvent)aciEvent;

                    if (evt.Command == AciOpCode.Bond)
                        _btTimer.IsStarted = true;
                    break;

                case AciEventType.Connected:
                    _btTimer.IsStarted = false;
                    _onboardLed.Write(true);
                    break;

                case AciEventType.Disconnected:
                    _btTimer.IsStarted = true;

                    if (_nrf.Bonded)
                        _nrf.AwaitConnection(BtLeBondTimeout, BtLeBondInterval);
                    break;                
            }
        }

        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (dataReceivedEvent.ServicePipeId == CommandPipeId)
                HandleCommand(dataReceivedEvent.Data);   
        }
    }
}
