using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NemesisModCompanion.UwpApp.Controls
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (bool)value;
            if (v)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var v = (Visibility)value;
            if (v == Visibility.Visible)
            {
                return true;
            }

            return false;
        }
    }
}