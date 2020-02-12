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
    class DurumRenkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnumMotorStatus durum = (EnumMotorStatus)value;
            if (durum == EnumMotorStatus.BaglantiYok)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            else if (durum == EnumMotorStatus.HataliVeri)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (durum == EnumMotorStatus.Run)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else
            {
                return new SolidColorBrush(Colors.Blue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
