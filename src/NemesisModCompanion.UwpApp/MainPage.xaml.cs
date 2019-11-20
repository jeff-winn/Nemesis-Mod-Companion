using System;
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
    }
}
