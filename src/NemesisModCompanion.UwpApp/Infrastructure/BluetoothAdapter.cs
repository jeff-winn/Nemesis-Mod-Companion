using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;
using Guid = System.Guid;

namespace NemesisModCompanion.UwpApp.Infrastructure
{
    public class BluetoothAdapter
    {
        private GattCharacteristic flywheelM1CurrentMilliamps;
        private GattCharacteristic flywheelM2CurrentMilliamps;
        private GattCharacteristic beltM1CurrentMilliamps;
        private GattCharacteristic flywheelSpeed;

        public static BluetoothAdapter Instance { get; } = new BluetoothAdapter();

        private DeviceInformation info;

        public event EventHandler<ValueChangedEventArgs<int>> FlywheelM1CurrentMilliampsChanged;

        public event EventHandler<ValueChangedEventArgs<int>> FlywheelM2CurrentMilliampsChanged;

        public event EventHandler<ValueChangedEventArgs<int>> BeltM1CurrentMilliampsChanged;

        public async Task ConnectAsync()
        {
            if (info.Pairing.CanPair)
            {
                await info.Pairing.PairAsync();
            }
        }

        public async Task ChangeFlywheelSpeed(int value)
        {
            byte b = (byte)value;

            var writer = new DataWriter();
            writer.WriteBytes(new[] { b });

            await flywheelSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task AttachToDevice()
        {
            var device = await BluetoothLEDevice.FromIdAsync(info.Id);
            if (device == null || device.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                return;
            }

            var accessResult = await device.RequestAccessAsync();
            if (accessResult != DeviceAccessStatus.Allowed)
            {
                return;
            }

            var servicesQueryResult = await device.GetGattServicesForUuidAsync(Guid.Parse("6817ff09-0000-95b0-47be-c4d08729f1f0"));

            var service = servicesQueryResult.Services.SingleOrDefault();
            if (service == null)
            {
                return;
            }

            var characteristics = await service.GetCharacteristicsAsync();

            flywheelSpeed = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000100-0000-95b0-47be-c4d08729f1f0"));

            flywheelM1CurrentMilliamps = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000107-0000-95b0-47be-c4d08729f1f0"));
            if (flywheelM1CurrentMilliamps != null)
            {
                await flywheelM1CurrentMilliamps.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                flywheelM1CurrentMilliamps.ValueChanged += OnFlywheelM1CurrentMilliampsChanged;
            }

            flywheelM2CurrentMilliamps = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000108-0000-95b0-47be-c4d08729f1f0"));
            if (flywheelM2CurrentMilliamps != null)
            {
                await flywheelM2CurrentMilliamps.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                flywheelM2CurrentMilliamps.ValueChanged += OnFlywheelM2CurrentMilliampsChanged;
            }

            beltM1CurrentMilliamps = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000109-0000-95b0-47be-c4d08729f1f0"));
            if (beltM1CurrentMilliamps != null)
            {
                await beltM1CurrentMilliamps.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                beltM1CurrentMilliamps.ValueChanged += OnBeltM1CurrentMilliampsChanged;
            }
        }

        private void OnFlywheelM1CurrentMilliampsChanged(object sender, GattValueChangedEventArgs e)
        {
            if (FlywheelM1CurrentMilliampsChanged == null)
            {
                return;
            }

            var bytes = e.CharacteristicValue.ToArray();
            var i = BitConverter.ToInt32(bytes, 0);

            FlywheelM1CurrentMilliampsChanged(this, new ValueChangedEventArgs<int>
            {
                Value = i,
                Timestamp = e.Timestamp
            });
        }

        private void OnFlywheelM2CurrentMilliampsChanged(object sender, GattValueChangedEventArgs e)
        {
            if (FlywheelM2CurrentMilliampsChanged == null)
            {
                return;
            }

            var bytes = e.CharacteristicValue.ToArray();
            var i = BitConverter.ToInt32(bytes, 0);

            FlywheelM2CurrentMilliampsChanged(this, new ValueChangedEventArgs<int>
            {
                Value = i,
                Timestamp = e.Timestamp
            });
        }

        private void OnBeltM1CurrentMilliampsChanged(object sender, GattValueChangedEventArgs e)
        {
            if (BeltM1CurrentMilliampsChanged == null)
            {
                return;
            }

            var bytes = e.CharacteristicValue.ToArray();
            var i = BitConverter.ToInt32(bytes, 0);

            BeltM1CurrentMilliampsChanged(this, new ValueChangedEventArgs<int>
            {
                Value = i,
                Timestamp = e.Timestamp
            });
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