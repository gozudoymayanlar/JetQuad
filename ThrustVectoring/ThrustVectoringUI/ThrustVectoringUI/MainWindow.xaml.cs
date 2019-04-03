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

using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using ThrustVectoringUI.ViewModels;
using ThrustVectoringUI.Views;



namespace ThrustVectoringUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KontrolPaneliView a = new KontrolPaneliView();
            this.Content = a;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double AspectRatio = (16.0 / 9.0);

            if (sizeInfo.WidthChanged)
            {
                this.Width = sizeInfo.NewSize.Height * AspectRatio;
            }
            else
            {
                this.Height = sizeInfo.NewSize.Width / AspectRatio;
            }
        }
    }
}
