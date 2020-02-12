using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AttitudeControlTestUI.Models;
using System.Windows.Media;

namespace AttitudeControlTestUI.Converters
{
    class RpmRenkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float rpm = (float)value;
            float rpmMax = 150000;
            float rpmMin = 20000;
            float renkMax = 160;
            float renkMin = 0;

            float hue = renkMax - (rpm - rpmMin) * (renkMax - renkMin) / (rpmMax - rpmMin);
            hue = hue > renkMax ? renkMax : hue;
            hue = hue < renkMin ? renkMin : hue;

            return new SolidColorBrush(HlsToRgbClass.HlsToRgb(hue, 0.4, 1));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}


