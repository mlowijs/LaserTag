using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace LaserTag.DesktopClient.Services
{
    public abstract class BtLeServiceBase
    {
        public const string ServiceUuidFormatString = "f0fa{0}-36a8-44b6-b21a-b518deea2c2d";

        protected GattDeviceService Service { get; set; }

        protected async Task EnableNotify(GattCharacteristic characteristic)
        {
            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
        }

        protected async Task<GattDeviceService> GetServiceByUuid(Guid uuid)
        {
            var deviceSelector = GattDeviceService.GetDeviceSelectorFromUuid(uuid);
            var deviceInfos = await DeviceInformation.FindAllAsync(deviceSelector);
            var deviceId = deviceInfos.FirstOrDefault()?.Id;

            if (deviceId == null)
                return null;

            var device = await BluetoothLEDevice.FromIdAsync(deviceId);

            return device.GetGattService(uuid);
        }


        public abstract Task InitializeAsync();
    }
}
