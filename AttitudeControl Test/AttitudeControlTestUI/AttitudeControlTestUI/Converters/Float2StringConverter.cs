using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;


namespace AttitudeControlTestUI.Converters
{
    class Float2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((float)value).ToString("#####.0#");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float a = 0;
            float.TryParse((string)value, out a);
            return a;
        }
    }
}
