using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ThrustVectoringUI.ViewModels;

namespace ThrustVectoringUI.Views
{
    /// <summary>
    /// Interaction logic for KontrolPaneliView.xaml
    /// </summary>
    public partial class KontrolPaneliView : UserControl
    {
        public KontrolPaneliView()
        {
            InitializeComponent();

            this.DataContext = new KontrolPaneliViewModel();
        }

        private void Slider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((((Slider)sender).Value + e.Delta / 100) <= ((Slider)sender).Maximum && 
                (((Slider)sender).Value + e.Delta / 100) >= ((Slider)sender).Minimum)
            {
                ((Slider)sender).Value += e.Delta / 100;
            }
        }


    }
}
