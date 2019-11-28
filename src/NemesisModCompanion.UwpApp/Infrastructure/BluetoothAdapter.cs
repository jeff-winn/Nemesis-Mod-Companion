using System;
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

            var servicesQueryResult = await device.GetGattServicesForUuidAsync(Guid.Parse("6817ff09-0000-95b0-47be-c4d08729f1f0"));

            var service = servicesQueryResult.Services.SingleOrDefault();
            if (service == null)
            {
                return;
            }

            var characteristics = await service.GetCharacteristicsAsync();

            flywheelSpeed = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000100-0000-95b0-47be-c4d08729f1f0"));
            flywheelM1TrimSpeed = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000105-0000-95b0-47be-c4d08729f1f0"));
            flywheelM2TrimSpeed = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000106-0000-95b0-47be-c4d08729f1f0"));
            beltSpeed = characteristics.Characteristics.SingleOrDefault(o => o.Uuid == Guid.Parse("00000101-0000-95b0-47be-c4d08729f1f0"));
            
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

        public int GetCurrentFlywheelSpeed()
        {
            var readResult = flywheelSpeed.ReadValueAsync().GetAwaiter().GetResult();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public float GetFlywheelM1TrimSpeed()
        {
            var readResult = flywheelM1TrimSpeed.ReadValueAsync().GetAwaiter().GetResult();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToSingle(bytes, 0);
        }

        public float GetFlywheelM2TrimSpeed()
        {
            var readResult = flywheelM2TrimSpeed.ReadValueAsync().GetAwaiter().GetResult();
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
            var m1ValueBytes = BitConverter.GetBytes(flywheelM1TrimValue);
            var m2ValueBytes = BitConverter.GetBytes(flywheelM2TrimValue);

            var writer = new DataWriter();
            writer.WriteBytes(m1ValueBytes);

            await flywheelM1TrimSpeed.WriteValueAsync(writer.DetachBuffer());

            writer = new DataWriter();
            writer.WriteBytes(m2ValueBytes);

            await flywheelM2TrimSpeed.WriteValueAsync(writer.DetachBuffer());
        }
    }
}