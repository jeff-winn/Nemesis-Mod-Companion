using NemesisModCompanion.Core.Domain.Bluetooth;
using System;
using Windows.UI.Xaml.Data;

namespace NemesisModCompanion.UwpApp.Controls
{
    class BeltSpeedToBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }

            var b = (BeltSpeed)(byte)value;
            var p = Enum.Parse<BeltSpeed>((string)parameter);

            return b == p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Enum.Parse<BeltSpeed>((string)parameter);
        }
    }
}
