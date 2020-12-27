using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using GalaSoft.MvvmLight;
using NemesisModCompanion.Core.Domain.Bluetooth;
using Windows.UI.Core;
using System.Diagnostics;

namespace NemesisModCompanion.UwpApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private CoreDispatcher dispatcher;
        private IBluetoothAdapter bluetoothAdapter;

        public void Initialize(CoreDispatcher dispatcher, IBluetoothAdapter bluetoothAdapter)
        {
            this.dispatcher = dispatcher;
            this.bluetoothAdapter = bluetoothAdapter;

            this.bluetoothAdapter.FlywheelM1CurrentMilliampsChanged += OnFlywheelM1CurrentMilliampsChanged;
            this.bluetoothAdapter.FlywheelM2CurrentMilliampsChanged += OnFlywheelM2CurrentMilliampsChanged;
            this.bluetoothAdapter.BeltM1CurrentMilliampsChanged += OnBeltM1CurrentMilliampsChanged;
        }

        private bool hopperLockEnabled;

        public bool HopperLockEnabled
        {
            get => hopperLockEnabled;
            set
            {
                if (hopperLockEnabled != value)
                {
                    hopperLockEnabled = value;
                    RaisePropertyChanged(nameof(HopperLockEnabled));
                }
            }
        }

        private int feedNormalSpeedValue;

        public int FeedNormalSpeedValue
        {
            get => feedNormalSpeedValue;
            set
            {
                if (feedNormalSpeedValue != value)
                {
                    feedNormalSpeedValue = value;
                    RaisePropertyChanged(nameof(FeedNormalSpeedValue));
                }
            }
        }

        private int feedMediumSpeedValue;

        public int FeedMediumSpeedValue
        {
            get => feedMediumSpeedValue;
            set
            {
                if (feedMediumSpeedValue != value)
                {
                    feedMediumSpeedValue = value;
                    RaisePropertyChanged(nameof(FeedMediumSpeedValue));
                }
            }
        }

        private int feedMaxSpeedValue;

        public int FeedMaxSpeedValue
        {
            get => feedMaxSpeedValue;
            set
            {
                if (feedMaxSpeedValue != value)
                {
                    feedMaxSpeedValue = value;
                    RaisePropertyChanged(nameof(FeedMaxSpeedValue));
                }
            }
        }

        private double flywheelTrimVariance;

        public double FlywheelTrimVarianceValue
        {
            get => flywheelTrimVariance;
            set
            {
                if (flywheelTrimVariance.Equals(value))
                {
                    flywheelTrimVariance = value;
                    RaisePropertyChanged(nameof(FlywheelTrimVarianceValue));
                }
            }
        }

        private int flywheelM1CurrentMilliamps;

        public int FlywheelM1CurrentMilliamps
        {
            get => flywheelM1CurrentMilliamps;
            set
            {
                if (flywheelM1CurrentMilliamps != value)
                {
                    flywheelM1CurrentMilliamps = value;
                    RaisePropertyChanged(nameof(FlywheelM1CurrentMilliamps));
                }
            }
        }

        private int flywheelM2CurrentMilliamps;

        public int FlywheelM2CurrentMilliamps
        {
            get => flywheelM2CurrentMilliamps;
            set
            {
                if (flywheelM2CurrentMilliamps != value)
                {
                    flywheelM2CurrentMilliamps = value;
                    RaisePropertyChanged(nameof(FlywheelM2CurrentMilliamps));
                }
            }
        }

        private FlywheelSpeed currentFlywheelSpeed;

        public FlywheelSpeed CurrentFlywheelSpeed
        {
            get => currentFlywheelSpeed;
            set
            {
                if (currentFlywheelSpeed != value)
                {
                    currentFlywheelSpeed = value;
                    RaisePropertyChanged(nameof(CurrentFlywheelSpeed));
                }
            }
        }

        private BeltSpeed currentBeltSpeed;

        public BeltSpeed CurrentBeltSpeed
        {
            get => currentBeltSpeed;
            set
            {
                if (currentBeltSpeed != value)
                {
                    currentBeltSpeed = value;
                    RaisePropertyChanged(nameof(CurrentBeltSpeed));
                }
            }
        }

        private int flywheelKidSpeed;

        public int FlywheelKidSpeedValue
        {
            get => flywheelKidSpeed;
            set
            {
                if (flywheelKidSpeed != value)
                {
                    flywheelKidSpeed = value;
                    RaisePropertyChanged(nameof(FlywheelKidSpeedValue));
                }
            }
        }

        private int flywheelNormalSpeed;

        public int FlywheelNormalSpeedValue
        {
            get => flywheelNormalSpeed;
            set
            {
                if (flywheelNormalSpeed != value)
                {
                    flywheelNormalSpeed = value;
                    RaisePropertyChanged(nameof(FlywheelNormalSpeedValue));
                }
            }
        }

        private int flywheelLudicrousSpeed;

        public int FlywheelLudicrousSpeedValue
        {
            get => flywheelLudicrousSpeed;
            set
            {
                if (flywheelLudicrousSpeed != value)
                {
                    flywheelLudicrousSpeed = value;
                    RaisePropertyChanged(nameof(FlywheelLudicrousSpeedValue));
                }
            }
        }

        private int beltM1CurrentMilliamps;

        public int BeltM1CurrentMilliamps
        {
            get => beltM1CurrentMilliamps;
            set
            {
                if (beltM1CurrentMilliamps != value)
                {
                    beltM1CurrentMilliamps = value;
                    RaisePropertyChanged(nameof(BeltM1CurrentMilliamps));
                }
            }
        }

        private double flywheelM1TrimValue;

        public double FlywheelM1TrimValue
        {
            get => flywheelM1TrimValue;
            set
            {
                if (!flywheelM1TrimValue.Equals(value))
                {
                    flywheelM1TrimValue = value;
                    RaisePropertyChanged(nameof(FlywheelM1TrimValue));
                }
            }
        }

        private double flywheelM2TrimValue;

        public double FlywheelM2TrimValue
        {
            get => flywheelM2TrimValue;
            set
            {
                if (!flywheelM2TrimValue.Equals(value))
                {
                    flywheelM2TrimValue = value;
                    RaisePropertyChanged(nameof(FlywheelM2TrimValue));
                }
            }
        }

        public bool IsAttached
        {
            get => bluetoothAdapter.IsAttached;
        }

        public void RaiseIsAttachedHasChanged()
        {
            RaisePropertyChanged(nameof(IsAttached));
        }

        public MainViewModel()
        {
        }

        public async Task Refresh()
        {
            CurrentFlywheelSpeed = await bluetoothAdapter.GetCurrentFlywheelSpeed();
            CurrentBeltSpeed = await bluetoothAdapter.GetCurrentBeltSpeed();
            FeedNormalSpeedValue = await bluetoothAdapter.GetBeltNormalSpeed();
            FeedMediumSpeedValue = await bluetoothAdapter.GetBeltMediumSpeed();
            FeedMaxSpeedValue = await bluetoothAdapter.GetBeltMaxSpeed();
            FlywheelM1TrimValue = await bluetoothAdapter.GetFlywheelM1TrimSpeed() * 1024;
            FlywheelM2TrimValue = await bluetoothAdapter.GetFlywheelM2TrimSpeed() * 1024;
            FlywheelKidSpeedValue = await bluetoothAdapter.GetFlywheelKidSpeed();
            FlywheelNormalSpeedValue = await bluetoothAdapter.GetFlywheelNormalSpeed();
            FlywheelLudicrousSpeedValue = await bluetoothAdapter.GetFlywheelLudicrousSpeed();
            FlywheelTrimVarianceValue = await bluetoothAdapter.GetFlywheelTrimVariance() * 100;
            HopperLockEnabled = await bluetoothAdapter.GetHopperLockEnabled();
        }

        private async void OnBeltM1CurrentMilliampsChanged(object sender, CurrentChangedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BeltM1CurrentMilliamps = e.Milliamps;
            });
        }

        private async void OnFlywheelM1CurrentMilliampsChanged(object sender, CurrentChangedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FlywheelM1CurrentMilliamps = e.Milliamps;
            });
        }

        private async void OnFlywheelM2CurrentMilliampsChanged(object sender, CurrentChangedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FlywheelM2CurrentMilliamps = e.Milliamps;
            });
        }
    }
}