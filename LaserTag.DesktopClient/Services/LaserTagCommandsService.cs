using LaserTag.Model;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace LaserTag.DesktopClient.Services
{
    public class LaserTagCommandsService : BtLeServiceBase
    {
        public static readonly Guid ServiceUuid = new Guid(string.Format(ServiceUuidFormatString, "2000"));

        private GattCharacteristic _commandChar;


        public override async Task InitializeAsync()
        {
            Service = await GetServiceByUuid(ServiceUuid);

            var characteristics = Service.GetAllCharacteristics();

            _commandChar = characteristics[0];
        }


        public async Task SetRedDotSightPower(bool enabled)
        {
            await _commandChar.WriteValueAsync(new byte[] { (byte)Command.RdsPower, (byte)(enabled ? 1 : 0) }.AsBuffer(), GattWriteOption.WriteWithoutResponse);
        }

        public async Task Reload()
        {
            await _commandChar.WriteValueAsync(new byte[] { (byte)Command.Reload }.AsBuffer(), GattWriteOption.WriteWithoutResponse);
        }

        public async Task Respawn()
        {
            await _commandChar.WriteValueAsync(new byte[] { (byte)Command.Respawn }.AsBuffer(), GattWriteOption.WriteWithoutResponse);
        }
    }
}
