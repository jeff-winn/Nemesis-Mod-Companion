﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace NemesisModCompanion.UwpApp.Infrastructure
{
    public class BluetoothAdapter
    {
        private GattCharacteristic flywheelM1CurrentMilliamps;
        private GattCharacteristic flywheelM2CurrentMilliamps;
        private GattCharacteristic beltM1CurrentMilliamps;
        private GattCharacteristic flywheelSpeed;
        private GattCharacteristic flywheelM1TrimSpeed;
        private GattCharacteristic flywheelM2TrimSpeed;
        private GattCharacteristic beltSpeed;

        private GattCharacteristic flywheelNormalSpeed;
        private GattCharacteristic flywheelKidSpeed;
        private GattCharacteristic flywheelLudicrousSpeed;
        private GattCharacteristic flywheelTrimVariance;
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

        public async Task ChangeFeedMaxSpeed(int value)
        {
            var writer = new DataWriter();
            writer.WriteInt32(value);

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

            beltMaxSpeed = characteristics.SingleOrDefault(o => o.UserDescription == "Belt Feed Max Speed");

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

        public async Task<int> GetBeltMaxSpeed()
        {
            var readResult = await beltMaxSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray().Reverse().ToArray();

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
            var bytes = readResult.Value.ToArray().Reverse().ToArray();

            return BitConverter.ToSingle(bytes, 0);
        }

        public async Task<float> GetFlywheelM2TrimSpeed()
        {
            var readResult = await flywheelM2TrimSpeed.ReadValueAsync();
            var bytes = readResult.Value.ToArray().Reverse().ToArray();

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

        public async Task ChangeTrimSpeeds(float flywheelM1TrimValue, float flywheelM2TrimValue)
        {
            var writer = new DataWriter();
            writer.WriteSingle(flywheelM1TrimValue);
            
            await flywheelM1TrimSpeed.WriteValueAsync(writer.DetachBuffer());

            writer = new DataWriter();
            writer.WriteSingle(flywheelM2TrimValue);

            await flywheelM2TrimSpeed.WriteValueAsync(writer.DetachBuffer());
        }
    }
}