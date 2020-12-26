using System;
using NemesisModCompanion.Core.Domain.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace NemesisModCompanion.UwpApp.Infrastructure.Bluetooth
{
    public class DiscoveryService : IDiscoveryService
    {
        public static readonly string[] RequestedProperties =
            {"System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.AepId"};

        private DeviceWatcher deviceWatcher;

        public event EventHandler<FoundDeviceEventArgs> FoundDevice; 

        public void Start(bool alreadyPaired)
        {
            // This needs to be changed based on whether the device has already been connected!
            deviceWatcher = DeviceInformation.CreateWatcher(
                    BluetoothLEDevice.GetDeviceSelectorFromPairingState(alreadyPaired), 
                    RequestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Start();
        }

        public void Stop()
        {
            deviceWatcher.Stop();
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            FoundDevice?.Invoke(this, new FoundDeviceEventArgs
            {
                Id = args.Id
            });
        }
    }
}