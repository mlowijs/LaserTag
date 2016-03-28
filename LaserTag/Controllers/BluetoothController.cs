using Nrf8001Driver;
using Nrf8001Driver.Events;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LaserTag.Controllers
{
    public class BluetoothController
    {
        #region nRF8001 Setup Data
        private readonly byte[][] SetupData = new byte[][]
        {
            new byte[] {0x00,0x00,0x03,0x02,0x42,0x07,},
            new byte[] {0x10,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x03,0x00,0x03,0x01,0x01,0x00,0x00,0x06,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0x10,0x1c,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x10,0x00,0x00,0x00,0x10,0x03,0x90,0x01,0xff,},
            new byte[] {0x10,0x38,0xff,0xff,0x02,0x58,0x00,0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0x10,0x54,0x01,0x00,},
            new byte[] {0x20,0x00,0x04,0x04,0x02,0x02,0x00,0x01,0x28,0x00,0x01,0x00,0x18,0x04,0x04,0x05,0x05,0x00,0x02,0x28,0x03,0x01,0x02,0x03,0x00,0x00,0x2a,0x04,0x04,0x14,},
            new byte[] {0x20,0x1c,0x0f,0x00,0x03,0x2a,0x00,0x01,0x4c,0x61,0x73,0x65,0x72,0x54,0x61,0x67,0x20,0x47,0x75,0x6e,0x20,0x30,0x31,0x00,0x00,0x00,0x00,0x00,0x04,0x04,},
            new byte[] {0x20,0x38,0x05,0x05,0x00,0x04,0x28,0x03,0x01,0x02,0x05,0x00,0x01,0x2a,0x06,0x04,0x03,0x02,0x00,0x05,0x2a,0x01,0x01,0xc0,0x03,0x04,0x04,0x05,0x05,0x00,},
            new byte[] {0x20,0x54,0x06,0x28,0x03,0x01,0x02,0x07,0x00,0x04,0x2a,0x06,0x04,0x09,0x08,0x00,0x07,0x2a,0x04,0x01,0x06,0x00,0x06,0x00,0x00,0x00,0xff,0xff,0x04,0x04,},
            new byte[] {0x20,0x70,0x02,0x02,0x00,0x08,0x28,0x00,0x01,0x01,0x18,0x04,0x04,0x10,0x10,0x00,0x09,0x28,0x00,0x01,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,},
            new byte[] {0x20,0x8c,0xa8,0x36,0x00,0x10,0xfa,0xf0,0x04,0x04,0x13,0x13,0x00,0x0a,0x28,0x03,0x01,0x12,0x0b,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,},
            new byte[] {0x20,0xa8,0xa8,0x36,0x01,0x10,0xfa,0xf0,0x16,0x0c,0x02,0x01,0x00,0x0b,0x10,0x01,0x02,0x00,0x46,0x34,0x03,0x02,0x00,0x0c,0x29,0x02,0x01,0x00,0x00,0x04,},
            new byte[] {0x20,0xc4,0x04,0x13,0x13,0x00,0x0d,0x28,0x03,0x01,0x12,0x0e,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x02,0x10,0xfa,0xf0,0x16,},
            new byte[] {0x20,0xe0,0x0c,0x02,0x01,0x00,0x0e,0x10,0x02,0x02,0x00,0x46,0x34,0x03,0x02,0x00,0x0f,0x29,0x02,0x01,0x00,0x00,0x04,0x04,0x10,0x10,0x00,0x10,0x28,0x00,},
            new byte[] {0x20,0xfc,0x01,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x00,0x20,0xfa,0xf0,0x04,0x04,0x13,0x13,0x00,0x11,0x28,0x03,0x01,0x0a,0x12,},
            new byte[] {0x21,0x18,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x01,0x20,0xfa,0xf0,0x46,0x3c,0x02,0x01,0x00,0x12,0x20,0x01,0x02,0x00,0x00,},
            new byte[] {0x40,0x00,0x10,0x01,0x02,0x00,0x02,0x04,0x00,0x0b,0x00,0x0c,0x10,0x02,0x02,0x00,0x02,0x04,0x00,0x0e,0x00,0x0f,0x20,0x01,0x02,0x04,0x00,0x04,0x00,0x12,},
            new byte[] {0x40,0x1c,0x00,0x00,},
            new byte[] {0x50,0x00,0x2d,0x2c,0xea,0xde,0x18,0xb5,0x1a,0xb2,0xb6,0x44,0xa8,0x36,0x00,0x00,0xfa,0xf0,},
            new byte[] {0x60,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0xf0,0x00,0x03,0x51,0x00,}
        };
        #endregion

        private const byte BtLeBondTimeout = 15;
        private const byte BtLeBondInterval = 64;

        private const byte AmmoStatPipeId = 1;
        private const byte ClipsStatPipeId = 2;
        private const byte HealthStatPipeId = 3;
        private const byte RdsPowerCommandPipeId = 4;
        private const byte ReloadCommandPipeId = 5;

        private Nrf8001 _nrf;

        public GameController GameController { get; set; }
        public IOController IOController { get; private set; }

        public BluetoothController(IOController ioController)
        {
            IOController = ioController;

            _nrf = new Nrf8001(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D7, SPI_Devices.SPI1);
            _nrf.AciEventReceived += OnAciEventReceived;
            _nrf.DataReceived += OnDataReceived;

            _nrf.Setup(SetupData);
            //_nrf.AwaitBond(BtLeBondTimeout, BtLeBondInterval);
        }


        public void Process()
        {
            _nrf.ProcessEvents();
        }


        public void NotifyAmmo(int newAmmo)
        {
            _nrf.SendData(AmmoStatPipeId, (byte)newAmmo);
        }

        public void NotifyClips(int newClips)
        {
            _nrf.SendData(ClipsStatPipeId, (byte)newClips);
        }

        public void NotifyHealth(int newHealth, byte shooterId)
        {
            _nrf.SendData(HealthStatPipeId, (byte)newHealth, shooterId);
        }


        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            switch (dataReceivedEvent.ServicePipeId)
            {
                case RdsPowerCommandPipeId:
                    IOController.RedDotSightEnabled = dataReceivedEvent.Data[0] == 0x01;
                    break;

                case ReloadCommandPipeId:
                    GameController.TryReloadGun();
                    break;
            }
        }


        private void OnAciEventReceived(AciEvent aciEvent)
        {
            if (aciEvent.EventType == AciEventType.Disconnected && _nrf.Bonded)
                _nrf.AwaitConnection(BtLeBondTimeout, BtLeBondInterval);
        }
    }
}