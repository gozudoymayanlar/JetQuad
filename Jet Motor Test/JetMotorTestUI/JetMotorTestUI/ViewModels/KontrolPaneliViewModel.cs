using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetMotorTestUI.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Globalization;

namespace JetMotorTestUI.ViewModels
{
    class KontrolPaneliViewModel : INotifyPropertyChanged
    {
        #region MVVM THINGIES
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChangedByExplicitName(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region PRIVATE FIELDS
        Stopwatch MyStopwatch = new Stopwatch();
        FileStream fs;
        StreamWriter sw;

        private ICommand _cmdBaglan;
        private ICommand _cmdMotorBaslat;
        private ICommand _cmdKaydet;
        private string dbPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\database\\";

        private bool _alreadySendingThrottle = false;
        #endregion

        public KontrolPaneliViewModel()
        {
            _cmdBaglan = new RelayCommand(CmdBaglanExecute, CmdBaglanCanExecute);
            _cmdMotorBaslat = new RelayCommand(CmdMotorBaslatExecute, CmdMotorBaslatCanExecute);
            _cmdKaydet = new RelayCommand(CmdKaydetExecute, CmdKaydetCanExecute);

            KontrolPaneli = new KontrolPaneliModel();
        }

        ~KontrolPaneliViewModel()
        {
            if (KontrolPaneli.MySerialPort != null)
            {
                if (KontrolPaneli.MySerialPort.IsOpen)
                {
                    KontrolPaneli.MySerialPort.DiscardInBuffer();
                    KontrolPaneli.MySerialPort.DiscardOutBuffer();
                    KontrolPaneli.MySerialPort.Close();
                }
            }
            MyStopwatch.Reset();
        }

        #region PUBLIC PROPERTIES
        public KontrolPaneliModel KontrolPaneli { get; set; }

        public ICommand CmdBaglan { get { return _cmdBaglan; } set { OnPropertyChanged(nameof(CmdBaglan)); } }
        public ICommand CmdMotorBaslat { get { return _cmdMotorBaslat; } set { OnPropertyChanged(nameof(CmdMotorBaslat)); } }
        public ICommand CmdKaydet { get { return _cmdKaydet; } set { OnPropertyChanged(nameof(CmdKaydet)); } }
        #endregion

        #region METHODS
        private bool CmdBaglanCanExecute(object arg)
        {
            if (KontrolPaneli.SeciliComPort == null || KontrolPaneli.SeciliComPort == "")
                return false;

            return true;
        }
        private void CmdBaglanExecute(object obj)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli)   // eğer bağlantı zaten varsa, bağlantıyı kes
            {
                KontrolPaneli.Timer.Stop();
                KontrolPaneli.Timer.Elapsed -= Timer_Elapsed;

                KontrolPaneli.MySerialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(serialPort_DataReceived);
                KontrolPaneli.MySerialPort.DiscardInBuffer();
                KontrolPaneli.MySerialPort.DiscardOutBuffer();
                KontrolPaneli.MySerialPort.Close();

                if (KontrolPaneli.KayitYap) // kayıt yapılıyorsa, durdur
                {
                    CmdKaydet.Execute(null);
                }
                KontrolPaneli.Baglanti = EnumBaglanti.BaglantiYok;
                MyStopwatch.Reset();
            }
            else    // eğer bağlantı yoksa bağlantı kurmaya çalış
            {
                try
                {
                    KontrolPaneli.MySerialPort = new SerialPort(KontrolPaneli.SeciliComPort, Convert.ToInt32(KontrolPaneli.Baudrate), Parity.None, 8, StopBits.One);
                    KontrolPaneli.MySerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort_DataReceived);
                    KontrolPaneli.MySerialPort.RtsEnable = true;
                    KontrolPaneli.MySerialPort.DtrEnable = true;
                    KontrolPaneli.MySerialPort.WriteTimeout = 1000;
                    KontrolPaneli.MySerialPort.Open();
                    KontrolPaneli.MySerialPort.DiscardInBuffer();

                    KontrolPaneli.Timer = new System.Timers.Timer(100);
                    KontrolPaneli.Timer.AutoReset = true;
                    KontrolPaneli.Timer.Elapsed += Timer_Elapsed;

                    KontrolPaneli.Baglanti = EnumBaglanti.Bagli;
                    MyStopwatch.Start();
                    KontrolPaneli.Timer.Start();
                    KontrolPaneli.MotorStatus = EnumMotorStatus.Standby_Start;


                }
                catch (Exception e)
                {
                    //throw;
                    MessageBox.Show(e.ToString());
                }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (KontrolPaneli.Baglanti == EnumBaglanti.BaglantiYok)
                return;

            if (!_alreadySendingThrottle)
            {
                _alreadySendingThrottle = true;
                try
                {
                    KontrolPaneli.MySerialPort.Write("\"" + KontrolPaneli.KumandaThrottle + ">!" + KontrolPaneli.KumandaTrim + ">");
                }
                catch (TimeoutException te)
                {
                    CmdBaglan.Execute(null);
                    MessageBox.Show(te.ToString(),"Arduya veri yazma belirlenen sürede tamamlanamadı.", MessageBoxButton.OK,
                        MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
                _alreadySendingThrottle = false;
            }
        }

        private bool CmdMotorBaslatCanExecute(object arg)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli && KontrolPaneli.MotorStatus != EnumMotorStatus.BaglantiYok)
                return true;

            return false;
        }
        private void CmdMotorBaslatExecute(object obj)
        {
            // TODO buraya motor başlatma throttle komutları sequence halinde yazılabilir ya da arduya yazılabilir burda sadece aktif edilir.
            KontrolPaneli.MotorStatus++;
        }

        private bool CmdKaydetCanExecute(object arg)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli)
                return true;

            return false;
        }
        private void CmdKaydetExecute(object obj)
        {
            if (KontrolPaneli.KayitYap)
            {
                KontrolPaneli.KayitYap = false;
                sw.Write("];" + Environment.NewLine + Environment.NewLine);
                sw.Close();
                fs.Close();
            }
            else
            {
                string s = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
                fs = new FileStream(dbPath + s + ".txt", FileMode.CreateNew);
                sw = new StreamWriter(fs, Encoding.Default);
                sw.Write("1.Time \r\n2.MotorStatus \r\n3.KumandaThrottle \r\n4.KumandaTrim \r\n5.ArduThrottle \r\n6.ArduTrim \r\n" +
                    "7.MotorThrottle \r\n8.Thrust \r\n9.RefRPM \r\n10.MotorRPM \r\n11.EGT \r\n12.BatteryVoltage \r\n13.PumpVoltage \r\n14.Fuel \r\n\r\n" + 
                    "veri" + s + "= [");
                KontrolPaneli.KayitYap = true;
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                MyStopwatch.Stop();
                KontrolPaneli.Time = MyStopwatch.ElapsedMilliseconds;
                MyStopwatch.Start();

                string receivedData = KontrolPaneli.MySerialPort.ReadLine();
                
                //%1#32#0#32#40#7300#7432#77#11.3#3.9#92
                if (receivedData[0] == '%') // if Angle datas are received
                {
                    receivedData = receivedData.Remove(0, 1);
                    string[] datas = receivedData.Split('#');

                    // BURADA DEĞİŞİKLİK YAPINCA ALTTA if (KontrolPaneli.KayitYap) İÇİNDE DEĞİŞİKLİK YAPMAYI UNUTMA
                    KontrolPaneli.MotorStatus = (EnumMotorStatus)(int.Parse(datas[0]));
                    KontrolPaneli.ArduThrottle = int.Parse(datas[1], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ArduTrim = int.Parse(datas[2], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.MotorThrottle = int.Parse(datas[3], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.Thrust = float.Parse(datas[4], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ReferenceRPM = long.Parse(datas[5], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.MotorRPM = long.Parse(datas[6], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.MotorRPM_gauge = KontrolPaneli.MotorRPM / 1000;
                    KontrolPaneli.EGT = int.Parse(datas[7], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.BatteryVoltage = float.Parse(datas[8], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.PumpVoltage = float.Parse(datas[9], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.Fuel = float.Parse(datas[10], CultureInfo.InvariantCulture.NumberFormat)/1000f;

                    KontrolPaneli.Baglanti = EnumBaglanti.Bagli;

                    if (KontrolPaneli.KayitYap)
                    {
                        // BURADA DEĞİŞİKLİK YAPINCA CmdKaydetExecute İÇİNDE SIRALAMAYI BELİRTEN YERDE DEĞİŞİKLİK YAPMAYI UNUTMA
                        sw.Write(KontrolPaneli.Time.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                (int) KontrolPaneli.MotorStatus+ "," +
                                KontrolPaneli.KumandaThrottle.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.KumandaTrim.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.ArduThrottle.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.ArduTrim.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.MotorThrottle.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.Thrust.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.ReferenceRPM.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.MotorRPM.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.EGT.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.BatteryVoltage.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.PumpVoltage.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.Fuel.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                                Environment.NewLine);
                    }
                }
                else
                {
                    KontrolPaneli.MySerialPort.DiscardInBuffer();
                    KontrolPaneli.Baglanti = EnumBaglanti.HataliVeri;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("ardudan data okuma işlemi yarım kaldı");
            }
        }

        public void ComPortlariBul()
        {
            KontrolPaneli.ComPortCollection = new ObservableCollection<string> { };
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length != 0)
            {
                foreach (string p in ports)
                {
                    KontrolPaneli.ComPortCollection.Add(p);
                }
            }
        }
        #endregion

    }
}

// timer kullandığım için burayı kullanmıyorum
//public void SendThrottle()
//{
//    if (KontrolPaneli.Baglanti == EnumBaglanti.BaglantiYok)
//        return;

//    if (!_alreadySendingThrottle)
//    {
//        _alreadySendingThrottle = true;
//        Task.Run(async () =>
//        {
//            await Task.Delay(100);
//            try
//            {
//                KontrolPaneli.MySerialPort.Write("\"" + KontrolPaneli.KumandaThrottle + ">!" + KontrolPaneli.KumandaTrim + ">");
//            }
//            catch (TimeoutException)
//            {
//                CmdBaglan.Execute(null);
//                MessageBox.Show("Arduya veri yazma belirlenen sürede tamamlanamadı.");
//            }
//            _alreadySendingThrottle = false;
//        });
//    }
//}

