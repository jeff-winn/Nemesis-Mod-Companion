﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NemesisModCompanion.UwpApp.Infrastructure;
using NemesisModCompanion.UwpApp.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NemesisModCompanion.UwpApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainViewModel vm;

        public MainPage()
        {
            InitializeComponent();

            vm = (MainViewModel)Resources["Vm"];
            vm.Initialize(Dispatcher);
        }

        private async void AttachButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.AttachToDevice();
        }

        private async void PairButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ConnectAsync();
        }

        private async void HighSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeFlywheelSpeed(255);
        }

        private async void NormalSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeFlywheelSpeed(1);
        }

        private async void MediumSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeFlywheelSpeed(2);
        }

        private async void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeTrimSpeeds(
                (float)(vm.FlywheelM1TrimValue / FlywheelM1TrimSlider.Maximum), 
                (float)(vm.FlywheelM2TrimValue / FlywheelM2TrimSlider.Maximum));
        }

        private async void NormalBeltSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeBeltSpeed(1);
        }

        private async void MediumBeltSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeBeltSpeed(2);
        }

        private async void HighBeltSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await BluetoothAdapter.Instance.ChangeBeltSpeed(255);
        }
    }
}
