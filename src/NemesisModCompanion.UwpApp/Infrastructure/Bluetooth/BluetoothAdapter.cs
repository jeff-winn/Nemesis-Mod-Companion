using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using NemesisModCompanion.Core.Domain.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

#pragma warning disable S1450

namespace NemesisModCompanion.UwpApp.Infrastructure.Bluetooth
{
    public class BluetoothAdapter : IBluetoothAdapter
    {
        private GattCharacteristic flywheelM1CurrentMilliamps;
        private GattCharacteristic flywheelM2CurrentMilliamps;
        private GattCharacteristic beltM1CurrentMilliamps;
        private GattCharacteristic flywheelSpeed;
        private GattCharacteristic flywheelM1TrimSpeed;
        private GattCharacteristic flywheelM2TrimSpeed;
        private GattCharacteristic beltSpeed;
        private GattCharacteristic hopperLockEnabled;

        private GattCharacteristic flywheelNormalSpeed;
        private GattCharacteristic flywheelKidSpeed;
        private GattCharacteristic flywheelLudicrousSpeed;
        private GattCharacteristic flywheelTrimVariance;

        private GattCharacteristic beltNormalSpeed;
        private GattCharacteristic beltMediumSpeed;
        private GattCharacteristic beltMaxSpeed;

        public static BluetoothAdapter Instance { get; } = new BluetoothAdapter();

        private DeviceInformation info;
        private BluetoothLEDevice device;

        public event EventHandler<ValueChangedEventArgs<int>> FlywheelM1CurrentMilliampsChanged;

        public event EventHandler<ValueChangedEventArgs<int>> FlywheelM2CurrentMilliampsChanged;

        public event EventHandler<ValueChangedEventArgs<int>> BeltM1CurrentMilliampsChanged;

        public bool IsAttached => device != null;

        public async Task ConnectAsync()
        {
            if (info.Pairing.CanPair)
            {
                await info.Pairing.PairAsync();
            }
        }

        public async Task ChangeFlywheelNormalSpeed(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await flywheelNormalSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeFlywheelKidSpeed(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await flywheelKidSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeFlywheelLudicrousSpeed(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await flywheelLudicrousSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeFeedNormalSpeed(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await beltNormalSpeed.WriteValueAsync(writer.DetachBuffer());
        }
        public async Task ChangeFeedMediumSpeed(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await beltMediumSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeFeedMaxSpeed(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await beltMaxSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeBeltSpeed(byte value)
        {
            var writer = new DataWriter();
            writer.WriteByte(value);

            await beltSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeFlywheelSpeed(byte value)
        {
            var writer = new DataWriter();
            writer.WriteByte(value);

            await flywheelSpeed.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task ChangeFlywheelTrimVariance(float value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await flywheelTrimVariance.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task AttachToDevice()
        {
            device = await BluetoothLEDevice.FromIdAsync(info.Id);
            if (device == null)
            {
                return;
            }

            var accessResult = await device.RequestAccessAsync();
            if (accessResult != DeviceAccessStatus.Allowed)
            {
                return;
            }

            await AttachBlasterServices();
            await AttachConfigurationServices();
            await AttachNotificationServices();
        }

        private async Task AttachBlasterServices()
        {
            var servicesQueryResult = await device.GetGattServicesForUuidAsync(Guid.Parse("6817ff09-0000-95b0-47be-c4d08729f1f0"));

            var service = servicesQueryResult.Services.SingleOrDefault();
            if (service == null)
            {
                return;
            }

            var characteristicsResult = await service.GetCharacteristicsAsync();
            var characteristics = characteristicsResult.Characteristics;

            foreach (var characteristic in characteristics)
            {
                Debug.WriteLine($"{characteristic.Uuid} - {characteristic.UserDescription}");
            }

            flywheelSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel Speed");
            beltSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Belt Speed");
            flywheelM1TrimSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel M1 Trim Speed");
            flywheelM2TrimSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel M2 Trim Speed");
        }

        private async Task AttachConfigurationServices()
        {
            var servicesQueryResult = await device.GetGattServicesForUuidAsync(Guid.Parse("6817ff09-0001-95b0-47be-c4d08729f1f0"));

            var service = servicesQueryResult.Services.SingleOrDefault();
            if (service == null)
            {
                return;
            }

            var characteristicsResult = await service.GetCharacteristicsAsync();
            var characteristics = characteristicsResult.Characteristics;

            foreach (var characteristic in characteristics)
            {
                Debug.WriteLine($"{characteristic.Uuid} - {characteristic.UserDescription}");
            }

            beltNormalSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Belt Feed Normal Speed");
            beltMediumSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Belt Feed Medium Speed");
            beltMaxSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Belt Feed Max Speed");
            hopperLockEnabled = characteristics.SingleOrDefault(o => o.UserDescription == "Hopper Lock Enabled");

            flywheelKidSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel Kid Speed");
            flywheelNormalSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel Normal Speed");
            flywheelLudicrousSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel Ludicrous Speed");
            flywheelTrimVariance = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel Trim Variance");
        }

        private async Task AttachNotificationServices()
        {
            var servicesQueryResult = await device.GetGattServicesForUuidAsync(Guid.Parse("6817ff09-0002-95b0-47be-c4d08729f1f0"));

            var service = servicesQueryResult.Services.SingleOrDefault();
            if (service == null)
            {
                return;
            }

            var characteristicsResult = await service.GetCharacteristicsAsync();
            var characteristics = characteristicsResult.Characteristics;

            foreach (var characteristic in characteristics)
            {
                Debug.WriteLine($"{characteristic.Uuid} - {characteristic.UserDescription}");
            }

            flywheelM1CurrentMilliamps = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel M1 Current (mA)");
            if (flywheelM1CurrentMilliamps != null)
            {
                await flywheelM1CurrentMilliamps.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                flywheelM1CurrentMilliamps.ValueChanged += OnFlywheelM1CurrentMilliampsChanged;
            }

            flywheelM2CurrentMilliamps = characteristics.SingleOrDefault(o => o.UserDescription == "Flywheel M2 Current (mA)");
            if (flywheelM2CurrentMilliamps != null)
            {
                await flywheelM2CurrentMilliamps.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                flywheelM2CurrentMilliamps.ValueChanged += OnFlywheelM2CurrentMilliampsChanged;
            }

            beltM1CurrentMilliamps = characteristics.SingleOrDefault(o => o.UserDescription == "Belt M1 Current (mA)");
            if (beltM1CurrentMilliamps != null)
            {
                await beltM1CurrentMilliamps.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                beltM1CurrentMilliamps.ValueChanged += OnBeltM1CurrentMilliampsChanged;
            }
        }

        public async Task ChangeHopperLockEnabled(bool value)
        {
            byte b = 0;
            if (value)
            {
                b = 1;
            }

            var writer = new DataWriter();
            writer.WriteByte(b);

            await hopperLockEnabled.WriteValueAsync(writer.DetachBuffer());
        }

        public async Task<bool> GetHopperLockEnabled()
        {
            var readResult = await hopperLockEnabled.ReadValueAsync();
            var value = readResult.Value.ToArray().Single();

            return value != 0;
        }

        public async Task<float> GetFlywheelTrimVariance()
        {
            var readResult = await flywheelTrimVariance.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToSingle(bytes, 0);
        }

        public async Task<int> GetBeltNormalSpeed()
        {
            var readResult = await beltNormalSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public async Task<int> GetBeltMediumSpeed()
        {
            var readResult = await beltMediumSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public async Task<int> GetBeltMaxSpeed()
        {
            var readResult = await beltMaxSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public async Task<int> GetCurrentFlywheelSpeed()
        {
            var readResult = await flywheelSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public async Task<int> GetFlywheelKidSpeed()
        {
            var readResult = await flywheelKidSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public async Task<int> GetFlywheelNormalSpeed()
        {
            var readResult = await flywheelNormalSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public async Task<int> GetFlywheelLudicrousSpeed()
        {
            var readResult = await flywheelLudicrousSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }


        public async Task<float> GetFlywheelM1TrimSpeed()
        {
            var readResult = await flywheelM1TrimSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToSingle(bytes, 0);
        }

        public async Task<float> GetFlywheelM2TrimSpeed()
        {
            var readResult = await flywheelM2TrimSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToSingle(bytes, 0);
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

        public void StartMonitoring()
        {
            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.AepId" };

            // This needs to be changed based on whether the device has already been connected!
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

        public async Task ChangeTrimSpeeds(float m1TrimValue, float m2TrimValue)
        {
            var bytes = BitConverter.GetBytes(m1TrimValue);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);
            
            await flywheelM1TrimSpeed.WriteValueAsync(writer.DetachBuffer());

            bytes = BitConverter.GetBytes(m2TrimValue);

            writer = new DataWriter();
            writer.WriteBytes(bytes);

            await flywheelM2TrimSpeed.WriteValueAsync(writer.DetachBuffer());
        }
    }
}