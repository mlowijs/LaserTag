using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace LaserTag.DesktopClient.Services
{
    public class LaserTagStatsService : BtLeServiceBase
    {
        public static readonly Guid ServiceUuid = new Guid(string.Format(ServiceUuidFormatString, "1000"));

        private GattCharacteristic _gunStatChar;
        private GattCharacteristic _playerStatChar;

        public event Action<int, int> GunChanged;
        public event Action<int, int> PlayerChanged;


        public override async Task InitializeAsync()
        {
            Service = await GetServiceByUuid(ServiceUuid);

            var characteristics = Service.GetAllCharacteristics();

            _gunStatChar = characteristics[0];
            _gunStatChar.ValueChanged += (c, e) => GunChanged(e.CharacteristicValue.ToArray()[0], e.CharacteristicValue.ToArray()[1]);
            await EnableNotify(_gunStatChar);

            _playerStatChar = characteristics[1];
            _playerStatChar.ValueChanged += (c, e) => PlayerChanged(e.CharacteristicValue.ToArray()[0], e.CharacteristicValue.ToArray()[1]);
            await EnableNotify(_playerStatChar);
        }
    }
}
