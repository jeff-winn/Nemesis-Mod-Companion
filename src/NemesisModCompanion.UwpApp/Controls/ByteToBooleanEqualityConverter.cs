using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NemesisModCompanion.UwpApp.Controls
{
    class ByteToBooleanEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }

            var b = (byte)value;
            var p = byte.Parse((string)parameter);

            return b == p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return byte.Parse((string)parameter);
        }
    }
}