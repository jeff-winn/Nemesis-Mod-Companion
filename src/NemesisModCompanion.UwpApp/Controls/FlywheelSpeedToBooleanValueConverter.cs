using NemesisModCompanion.Core.Domain.Bluetooth;
using System;
using Windows.UI.Xaml.Data;

namespace NemesisModCompanion.UwpApp.Controls
{
    class FlywheelSpeedToBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }

            var b = (FlywheelSpeed)(byte)value;
            var p = Enum.Parse<FlywheelSpeed>((string)parameter);

            return b == p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Enum.Parse<FlywheelSpeed>((string)parameter);
        }
    }
}