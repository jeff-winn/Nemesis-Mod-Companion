using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using NemesisModCompanion.Core.Domain.Bluetooth;
using NemesisModCompanion.UwpApp.Infrastructure;
using Windows.UI.Core;

namespace NemesisModCompanion.UwpApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private CoreDispatcher dispatcher;

        public void Initialize(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
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
            get => BluetoothAdapter.Instance.IsAttached;
        }

        public void RaiseIsAttachedHasChanged()
        {
            RaisePropertyChanged(nameof(IsAttached));
        }

        public MainViewModel()
        {
            BluetoothAdapter.Instance.FlywheelM1CurrentMilliampsChanged += OnFlywheelM1CurrentMilliampsChanged;
            BluetoothAdapter.Instance.FlywheelM2CurrentMilliampsChanged += OnFlywheelM2CurrentMilliampsChanged;
            BluetoothAdapter.Instance.BeltM1CurrentMilliampsChanged += OnBeltM1CurrentMilliampsChanged;
        }

        public async Task Refresh()
        {
            FeedNormalSpeedValue = await BluetoothAdapter.Instance.GetBeltNormalSpeed();
            FeedMediumSpeedValue = await BluetoothAdapter.Instance.GetBeltMediumSpeed();
            FeedMaxSpeedValue = await BluetoothAdapter.Instance.GetBeltMaxSpeed();
            FlywheelM1TrimValue = await BluetoothAdapter.Instance.GetFlywheelM1TrimSpeed() * 1024;
            FlywheelM2TrimValue = await BluetoothAdapter.Instance.GetFlywheelM2TrimSpeed() * 1024;
            FlywheelKidSpeedValue = await BluetoothAdapter.Instance.GetFlywheelKidSpeed();
            FlywheelNormalSpeedValue = await BluetoothAdapter.Instance.GetFlywheelNormalSpeed();
            FlywheelLudicrousSpeedValue = await BluetoothAdapter.Instance.GetFlywheelLudicrousSpeed();
            FlywheelTrimVarianceValue = await BluetoothAdapter.Instance.GetFlywheelTrimVariance() * 100;
            HopperLockEnabled = await BluetoothAdapter.Instance.GetHopperLockEnabled();
        }

        private async void OnBeltM1CurrentMilliampsChanged(object sender, ValueChangedEventArgs<int> e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BeltM1CurrentMilliamps = e.Value;
            });
        }

        private async void OnFlywheelM1CurrentMilliampsChanged(object sender, ValueChangedEventArgs<int> e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FlywheelM1CurrentMilliamps = e.Value;
            });
        }

        private async void OnFlywheelM2CurrentMilliampsChanged(object sender, ValueChangedEventArgs<int> e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FlywheelM2CurrentMilliamps = e.Value;
            });
        }
    }
}