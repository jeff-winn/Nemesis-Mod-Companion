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

        public static IBluetoothAdapter Instance { get; } = new BluetoothAdapter();

        private DeviceInformation info;
        private BluetoothLEDevice device;

        public event EventHandler<CurrentChangedEventArgs> FlywheelM1CurrentMilliampsChanged;

        public event EventHandler<CurrentChangedEventArgs> FlywheelM2CurrentMilliampsChanged;

        public event EventHandler<CurrentChangedEventArgs> BeltM1CurrentMilliampsChanged;

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
            await flywheelNormalSpeed.WriteInt32Async(value);
        }

        public async Task ChangeFlywheelKidSpeed(int value)
        {
            await flywheelKidSpeed.WriteInt32Async(value);
        }

        public async Task ChangeFlywheelLudicrousSpeed(int value)
        {
            await flywheelLudicrousSpeed.WriteInt32Async(value);
        }

        public async Task ChangeFeedNormalSpeed(int value)
        {
            await beltNormalSpeed.WriteInt32Async(value);
        }
        public async Task ChangeFeedMediumSpeed(int value)
        {
            await beltMediumSpeed.WriteInt32Async(value);
        }

        public async Task ChangeFeedMaxSpeed(int value)
        {
            await beltMaxSpeed.WriteInt32Async(value);
        }

        public async Task ChangeBeltSpeed(byte value)
        {
            await beltSpeed.WriteByteAsync(value);
        }

        public async Task ChangeFlywheelSpeed(byte value)
        {
            await flywheelSpeed.WriteByteAsync(value);
        }

        public async Task ChangeFlywheelTrimVariance(float value)
        {
            await flywheelTrimVariance.WriteFloatAsync(value);
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
            return await hopperLockEnabled.ReadBoolAsync();
        }

        public async Task<float> GetFlywheelTrimVariance()
        {
            return await flywheelTrimVariance.ReadFloatAsync();;
        }

        public async Task<int> GetBeltNormalSpeed()
        {
            return await beltNormalSpeed.ReadInt32Async();
        }

        public async Task<int> GetBeltMediumSpeed()
        {
            return await beltMediumSpeed.ReadInt32Async();
        }

        public async Task<int> GetBeltMaxSpeed()
        {
            return await beltMaxSpeed.ReadInt32Async();
        }

        public async Task<byte> GetCurrentBeltSpeed()
        {
            return await beltSpeed.ReadByteAsync();
        }

        public async Task<byte> GetCurrentFlywheelSpeed()
        {
            return await flywheelSpeed.ReadByteAsync();            
        }

        public async Task<int> GetFlywheelKidSpeed()
        {
            return await flywheelKidSpeed.ReadInt32Async();
        }

        public async Task<int> GetFlywheelNormalSpeed()
        {
            return await flywheelNormalSpeed.ReadInt32Async();
        }

        public async Task<int> GetFlywheelLudicrousSpeed()
        {
            return await flywheelLudicrousSpeed.ReadInt32Async();
        }

        public async Task<float> GetFlywheelM1TrimSpeed()
        {
            return await flywheelM1TrimSpeed.ReadFloatAsync();
        }

        public async Task<float> GetFlywheelM2TrimSpeed()
        {
            return await flywheelM2TrimSpeed.ReadFloatAsync();
        }

        private void OnFlywheelM1CurrentMilliampsChanged(object sender, GattValueChangedEventArgs e)
        {
            if (FlywheelM1CurrentMilliampsChanged == null)
            {
                return;
            }

            var bytes = e.CharacteristicValue.ToArray();
            var i = BitConverter.ToInt32(bytes, 0);

            FlywheelM1CurrentMilliampsChanged(this, new CurrentChangedEventArgs
            {
                Milliamps = i,
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

            FlywheelM2CurrentMilliampsChanged(this, new CurrentChangedEventArgs
            {
                Milliamps = i,
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

            BeltM1CurrentMilliampsChanged(this, new CurrentChangedEventArgs
            {
                Milliamps = i,
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