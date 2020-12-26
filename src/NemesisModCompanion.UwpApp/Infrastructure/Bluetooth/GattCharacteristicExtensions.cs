using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace NemesisModCompanion.UwpApp.Infrastructure.Bluetooth
{
    static class GattCharacteristicExtensions
    {
        public static async Task WriteInt32Async(this GattCharacteristic characteristic, int value)
        {
            var bytes = BitConverter.GetBytes(value);

            var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await characteristic.WriteValueAsync(writer.DetachBuffer());
        }

        public static async Task WriteFloatAsync(this GattCharacteristic characteristic, float value)
        {
            var bytes = BitConverter.GetBytes(value);

            using var writer = new DataWriter();
            writer.WriteBytes(bytes);

            await characteristic.WriteValueAsync(writer.DetachBuffer());
        }

        public static async Task WriteByteAsync(this GattCharacteristic characteristic, byte value)
        {
            var writer = new DataWriter();
            writer.WriteByte(value);

            await characteristic.WriteValueAsync(writer.DetachBuffer());
        }

        public static async Task<float> ReadFloatAsync(this GattCharacteristic characteristic)
        {
            var readResult = await characteristic.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToSingle(bytes, 0);
        }

        public static async Task<int> ReadInt32Async(this GattCharacteristic characteristic)
        {
            var readResult = await characteristic.ReadValueAsync();
            var bytes = readResult.Value.ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public static async Task<byte> ReadByteAsync(this GattCharacteristic characteristic)
        {
            var readResult = await characteristic.ReadValueAsync();
            return readResult.Value.ToArray().Single();
        }

        public static async Task<bool> ReadBoolAsync(this GattCharacteristic characteristic)
        {
            var readResult = await characteristic.ReadValueAsync();
            var value = readResult.Value.ToArray().Single();

            return value != 0;
        }
    }
}