using Microchip25xx080Driver;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LaserTag.Gun.Controllers
{
    public class FlashController
    {
        private const ushort GunIdAddress = 0x0000;

        private Mc25xx080 _mc25;

        public FlashController()
        {
            _mc25 = new Mc25xx080(16, Pins.GPIO_PIN_D3, SPI.SPI_module.SPI1);

            var gunId = _mc25.Read(GunIdAddress);

            if (gunId == 0x00 || gunId == 0xFF)
                _mc25.Write(GunIdAddress, 0x01);
        }


        public byte GetGunId()
        {
            var gunId = _mc25.Read(GunIdAddress);

            return gunId;
        }
    }
}
