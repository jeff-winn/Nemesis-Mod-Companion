using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace NemesisModCompanion.UwpApp.Infrastructure
{
    public class BluetoothAdapter
    {
        public static BluetoothAdapter Instance { get; } = new BluetoothAdapter();

        private DeviceInformation info;

        public async Task ConnectAsync()
        {
            if (info.Pairing.CanPair)
            {
                await info.Pairing.PairAsync();
            }
        }

        public async Task GetDevice()
        {
            var device = await BluetoothLEDevice.FromIdAsync(info.Id);
            if (device != null)
            {
                var servicesQueryResult = await device.GetGattServicesAsync();

                foreach (var service in servicesQueryResult.Services)
                {
                    var characteristicsQueryResult = await service.GetCharacteristicsAsync();

                    foreach (var characteristic in characteristicsQueryResult.Characteristics)
                    {
                        if (characteristic != null)
                        {
                            Debug.WriteLine(characteristic.Uuid.ToString());
                        }
                    }
                }
            }
        }

        public void Go()
        {

            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.AepId" };

            DeviceWatcher deviceWatcher =
                DeviceInformation.CreateWatcher(
                    BluetoothLEDevice.GetDeviceSelectorFromPairingState(true),
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Start();
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var name = args.Name;
            Debug.WriteLine(name);

            if (name.StartsWith("Nerf"))
            {
                info = args;
            }
        }
    }
}