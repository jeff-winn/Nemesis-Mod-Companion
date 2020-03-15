using NemesisModCompanion.Core.Domain.Bluetooth;
using NemesisModCompanion.UwpApp.Infrastructure.Bluetooth;
using NemesisModCompanion.UwpApp.ViewModels;
using Windows.UI.Xaml;

namespace NemesisModCompanion.UwpApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly MainViewModel vm;
        private readonly IBluetoothAdapter bluetoothAdapter;

        public MainPage()
        {
            InitializeComponent();

            vm = (MainViewModel)Resources["Vm"];
            bluetoothAdapter = BluetoothAdapter.Instance;

            vm.Initialize(Dispatcher, bluetoothAdapter);
        }

        private async void AttachButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.AttachToDevice();
            await vm.Refresh();

            vm.RaiseIsAttachedHasChanged();
        }

        private async void PairButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ConnectAsync();
        }

        private async void HighSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeFlywheelSpeed(255);
        }

        private async void NormalSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeFlywheelSpeed(1);
        }

        private async void MediumSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeFlywheelSpeed(2);
        }

        private async void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeTrimSpeeds(
                (float)(vm.FlywheelM1TrimValue / FlywheelM1TrimSlider.Maximum), 
                (float)(vm.FlywheelM2TrimValue / FlywheelM2TrimSlider.Maximum));

            await bluetoothAdapter.ChangeFeedNormalSpeed(
                vm.FeedNormalSpeedValue);

            await bluetoothAdapter.ChangeFeedMediumSpeed(
                vm.FeedMediumSpeedValue);

            await bluetoothAdapter.ChangeFeedMaxSpeed(
                vm.FeedMaxSpeedValue);

            await bluetoothAdapter.ChangeFlywheelKidSpeed(
                vm.FlywheelKidSpeedValue);

            await bluetoothAdapter.ChangeFlywheelNormalSpeed(
                vm.FlywheelNormalSpeedValue);

            await bluetoothAdapter.ChangeFlywheelLudicrousSpeed(
                vm.FlywheelLudicrousSpeedValue);

            await bluetoothAdapter.ChangeFlywheelTrimVariance(
                (float)(vm.FlywheelTrimVarianceValue / FlywheelTrimVarianceSlider.Maximum));

            await bluetoothAdapter.ChangeHopperLockEnabled(
                vm.HopperLockEnabled);
        }

        private async void NormalBeltSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeBeltSpeed(1);
        }

        private async void MediumBeltSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeBeltSpeed(2);
        }

        private async void HighBeltSpeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            await bluetoothAdapter.ChangeBeltSpeed(255);
        }
    }
}
