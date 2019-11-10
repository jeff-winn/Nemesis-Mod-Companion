using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

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
                var accessResult = await device.RequestAccessAsync();
                if (accessResult != DeviceAccessStatus.Allowed)
                {
                    return;
                }

                var servicesQueryResult = await device.GetGattServicesForUuidAsync(Guid.Parse("6817ff09-0001-95b0-47be-c4d08729f1f0"));

                foreach (var service in servicesQueryResult.Services)
                {
                    var serviceUuid = service.Uuid.ToString();
                    Debug.WriteLine($"Service: {serviceUuid}");

                    var characteristicsQueryResult = await service.GetCharacteristicsForUuidAsync(Guid.Parse("00000100-0001-95b0-47be-c4d08729f1f0"));
                    
                    foreach (var characteristic in characteristicsQueryResult.Characteristics)
                    {
                        Debug.WriteLine($"Characteristic: {characteristic.Uuid}");

                        var value = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                        var bytes = value.Value.ToArray();

                        PrintInt32FromArduino(bytes, "Before");

                        var n = 125;
                        bytes = BitConverter.GetBytes(n);

                        var writer = new DataWriter();
                        writer.WriteBytes(bytes);

                        var writeResult = await characteristic.WriteValueAsync(writer.DetachBuffer());

                        value = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                        bytes = value.Value.ToArray();

                        PrintInt32FromArduino(bytes, "After");
                    }
                }
            }
        }

        private void PrintInt32FromArduino(byte[] bytes, string message)
        {
            var n = BitConverter.ToInt32(bytes, 0);

            Debug.WriteLine($"{message}: {n}");
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