using System;
using GalaSoft.MvvmLight;
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

        public MainViewModel()
        {
            BluetoothAdapter.Instance.FlywheelM1CurrentMilliampsChanged += OnFlywheelM1CurrentMilliampsChanged;
            BluetoothAdapter.Instance.FlywheelM2CurrentMilliampsChanged += OnFlywheelM2CurrentMilliampsChanged;
            BluetoothAdapter.Instance.BeltM1CurrentMilliampsChanged += OnBeltM1CurrentMilliampsChanged;
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