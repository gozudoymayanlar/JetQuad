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
    class EgtRenkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float egt = (float)value;
            float egtMax = 1000;
            float egtMin = -20;
            float renkMax = 160;
            float renkMin = 0;

            float hue = renkMax - (egt - egtMin) * (renkMax - renkMin) / (egtMax - egtMin);
            hue = hue > renkMax ? renkMax : hue;
            hue = hue < renkMin ? renkMin : hue;

            return new SolidColorBrush(HlsToRgbClass.HlsToRgb(hue, 0.4, 1));


            //if (servoSicaklik > 60)
            //{
            //    return new SolidColorBrush(Colors.Red);
            //}
            //else if (servoSicaklik < 10)
            //{
            //    return new SolidColorBrush(Colors.Cyan);
            //}
            //else
            //{
                //float a1 = 247.4f; float b1 = 59.87f; float c1 = 14.66f; float a2 = 182.4f; float b2 = 42.14f; float c2 = 9.916f;
                //int Kirmizi = (int)(a1 * Math.Exp(-Math.Pow(((servoSicaklik - b1) / c1), 2)) + a2 * Math.Exp(-Math.Pow(((servoSicaklik - b2) / c2), 2)));
                //Kirmizi = Kirmizi > 255 ? 255 : Kirmizi;
                //Kirmizi = Kirmizi < 0 ? 0 : Kirmizi;

                //float p1 = -0.006167f;
                //float p2 = 0.4276f;
                //float p3 = -8.79f;
                //float p4 = 307.7f;
                //int Yesil = (int)(p1 * Math.Pow(servoSicaklik, 3) + p2 * Math.Pow(servoSicaklik, 2) + p3 * servoSicaklik + p4);
                //Yesil = Yesil > 255 ? 255 : Yesil;
                //Yesil = Yesil < 0 ? 0 : Yesil;

                //p1 = -0.006143f;
                //p2 = 0.865f;
                //p3 = -39.51f;
                //p4 = 582.4f;
                //int Mavi = (int)(p1 * Math.Pow(servoSicaklik, 3) + p2 * Math.Pow(servoSicaklik, 2) + p3 * servoSicaklik + p4);
                //Mavi = Mavi > 255 ? 255 : Mavi;
                //Mavi = Mavi < 0 ? 0 : Mavi;

                //return new SolidColorBrush(Color.FromRgb((byte)Kirmizi, (byte)Yesil, (byte)Mavi));
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}


