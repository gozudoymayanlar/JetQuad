using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttitudeControlTestUI.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Globalization;

using System.Windows.Media;
using LiveCharts.Configurations;
using System.Windows.Controls;

namespace AttitudeControlTestUI.ViewModels
{
    class KontrolPaneliViewModel : BaseViewModel
    {
        #region PRIVATE FIELDS
        Stopwatch MyStopwatch = new Stopwatch();
        FileStream fs;
        StreamWriter sw;

        private ICommand _cmdBaglan;
        private ICommand _cmdMotorBaslat;
        private ICommand _cmdKaydet;
        private ICommand _cmdEstop;
        private ICommand _cmdEnter;

        private string dbPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\database\\";

        private bool _alreadySendingThrottle = false;

        char[] initChars = { '"', '!', '\'', '^', '+', '%', '&', '/', '(', ')' };

        //private double _step;
        //private double[] _labels;
        //private Func<double, string> _formatter;
        #endregion

        public KontrolPaneliViewModel()
        {
            _cmdBaglan = new RelayCommand(CmdBaglanExecute, CmdBaglanCanExecute);
            _cmdMotorBaslat = new RelayCommand(CmdMotorBaslatExecute, CmdMotorBaslatCanExecute);
            _cmdKaydet = new RelayCommand(CmdKaydetExecute, CmdKaydetCanExecute);
            _cmdEstop = new RelayCommand(CmdEstopExecute, CmdEstopCanExecute);
            _cmdEnter = new RelayCommand(CmdEnterExecute, CmdEnterCanExecute);

            KontrolPaneli = new KontrolPaneliModel();
            

            //var dayConfig = Mappers.Xy<DateTimePoint>()
            //    .X(dateTimePoint => (double)dateTimePoint.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
            //    .Y(dateTimePoint => dateTimePoint.Value);

            //var dayConfig = Mappers.Xy<DateTimePoint>()
            //    .X(dateTimePoint => dateTimePoint.DateTime.ToOADate())
            //    .Y(dateTimePoint => dateTimePoint.Value);

            

            var dayConfig = Mappers.Xy<DateTimePoint>()
                .X(dateTimePoint => dateTimePoint.DateTime.Ticks)
                .Y(dateTimePoint => dateTimePoint.Value);

            #region grafik gösterme kodları
            //KontrolPaneli.RollSeriesCollection = new SeriesCollection(dayConfig)
            //{
            //    new LineSeries
            //    {
            //        Title = "Roll",
            //        //DataLabels = true,
            //        //Values = new ChartValues<DateTimePoint> { }

            //        Values = new ChartValues<DateTimePoint>
            //        {
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate,
            //                Value = 0
            //            },
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate.AddSeconds(2),
            //                Value = 0
            //            },
            //        }
            //    },
            //    new LineSeries
            //    {
            //        Title = "Roll Ref",
            //        //DataLabels = true,
            //        Values = new ChartValues<DateTimePoint>
            //        {
            //             new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate,
            //                Value = 0
            //            },
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate.AddSeconds(2),
            //                Value = 0
            //            }
            //        }
            //    }
            //};
            //KontrolPaneli.PitchSeriesCollection = new SeriesCollection(dayConfig)
            //{
            //    new LineSeries
            //    {
            //        Title = "Pitch",
            //        //DataLabels = true,
            //        Values = new ChartValues<DateTimePoint>
            //        {
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate,
            //                Value = 0
            //            },
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate.AddSeconds(2),
            //                Value = 0
            //            }
            //        }
            //    },
            //    new LineSeries
            //    {
            //        Title = "Pitch Ref",
            //        //DataLabels = true,
            //        Values = new ChartValues<DateTimePoint>
            //        {
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate,
            //                Value = 0
            //            },
            //            new DateTimePoint
            //            {
            //                DateTime = KontrolPaneli.StartingDate.AddSeconds(2),
            //                Value = 0
            //            }
            //        }
            //    }
            //};
            #endregion

            //Step = TimeSpan.FromSeconds(3).Ticks;
            //Labels = new double[] { -45, -30, -15, 0, 15, 30, 45 };
            //Formatter = value => new DateTime((long)value).ToString("mm:ss");
            //Formatter = value => new DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("t");
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
        public ICommand CmdEstop { get { return _cmdEstop; } set { OnPropertyChanged(nameof(CmdEstop)); } }
        public ICommand CmdEnter { get { return _cmdEnter; } set { OnPropertyChanged(nameof(CmdEnter)); } }


        //public Func<double, string> Formatter
        //{
        //    get { return _formatter; }
        //    set { _formatter = value; OnPropertyChanged(nameof(Formatter)); }
        //}

        //public double Step
        //{
        //    get { return _step; }
        //    set { _step = value; OnPropertyChanged(nameof(Step)); }
        //}

        //public double[] Labels
        //{
        //    get
        //    {
        //        return _labels;
        //    }

        //    set
        //    {
        //        _labels = value; OnPropertyChanged(nameof(Labels));
        //    }
        //}
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
                KontrolPaneli.MySerialPort.DiscardInBuffer();
                KontrolPaneli.MySerialPort.DiscardOutBuffer();

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
                    KontrolPaneli.Durum[0] = EnumMotorStatus.Standby_Start;
                    KontrolPaneli.Durum[1] = EnumMotorStatus.Standby_Start;
                    KontrolPaneli.Durum[2] = EnumMotorStatus.Standby_Start;
                    KontrolPaneli.Durum[3] = EnumMotorStatus.Standby_Start;
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
                    //KontrolPaneli.MySerialPort.Write("\"" + KontrolPaneli.BaslangicKutle + ">!" + KontrolPaneli.TahminiKutle + ">");
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
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli && KontrolPaneli.Durum[0] != EnumMotorStatus.BaglantiYok)
                return true;

            return false;
        }
        private void CmdMotorBaslatExecute(object obj)
        {
            // TODO buraya motor başlatma throttle komutları sequence halinde yazılabilir ya da arduya yazılabilir burda sadece aktif edilir.
            KontrolPaneli.Durum[1]++;
            MyStopwatch.Stop();
            KontrolPaneli.Time = MyStopwatch.ElapsedMilliseconds;
            MyStopwatch.Start();
            Random r = new Random();
            //KontrolPaneli.RollSeriesCollection[0].Values.Add(new DateTimePoint(KontrolPaneli.StartingDate.AddMilliseconds(KontrolPaneli.Time),r.Next(-60,60)));
            //KontrolPaneli.RollSeriesCollection[1].Values.Add(new DateTimePoint(KontrolPaneli.StartingDate.AddMilliseconds(KontrolPaneli.Time), r.Next(-60, 60)));

            //KontrolPaneli.PitchSeriesCollection[0].Values.Add(new DateTimePoint(KontrolPaneli.StartingDate.AddMilliseconds(KontrolPaneli.Time), r.Next(-60, 60)));
            //KontrolPaneli.PitchSeriesCollection[1].Values.Add(new DateTimePoint(KontrolPaneli.StartingDate.AddMilliseconds(KontrolPaneli.Time), r.Next(-60, 60)));

            //KontrolPaneli.Roll1Act = r.Next(-60, 60);
            //KontrolPaneli.Roll1Ref = r.Next(-60, 60);

            //KontrolPaneli.CurrentPitch = r.Next(-60, 60);
            //KontrolPaneli.CurrentPitchRef = r.Next(-60, 60);


            //while (((DateTimePoint)KontrolPaneli.RollSeriesCollection[0].Values[KontrolPaneli.RollSeriesCollection[0].Values.Count - 1]).DateTime.Ticks - ((DateTimePoint)KontrolPaneli.RollSeriesCollection[0].Values[0]).DateTime.Ticks > TimeSpan.FromSeconds(1).Ticks
            //    && KontrolPaneli.RollSeriesCollection[0].Values.Count > 2)
            //{
            //    KontrolPaneli.RollSeriesCollection[0].Values.RemoveAt(0);
            //    KontrolPaneli.RollSeriesCollection[1].Values.RemoveAt(0);

            //    KontrolPaneli.PitchSeriesCollection[0].Values.RemoveAt(0);
            //    KontrolPaneli.PitchSeriesCollection[1].Values.RemoveAt(0);
            //}

            //if (KontrolPaneli.RollSeriesCollection[0].Values.Count > 10)
            //{
            //    KontrolPaneli.RollSeriesCollection[0].Values.RemoveAt(0);
            //    KontrolPaneli.RollSeriesCollection[1].Values.RemoveAt(0);

            //    KontrolPaneli.PitchSeriesCollection[0].Values.RemoveAt(0);
            //    KontrolPaneli.PitchSeriesCollection[1].Values.RemoveAt(0);
            //}

            //KontrolPaneli.ServoError = r.Next(-1, 2);

        }

        private bool CmdKaydetCanExecute(object arg)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli)
                return true;

            return false;
        }

        // TODO - BURASI GÜNCELLENECEK
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
                // TODO ---- BURASI GÜNCELLENECEK !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                sw.Write("1.Time \r\n2.MotorStatus \r\n3.KumandaThrottle \r\n4.KumandaTrim \r\n5.ArduThrottle \r\n6.ArduTrim \r\n" +
                    "7.MotorThrottle \r\n8.Thrust \r\n9.RefRPM \r\n10.MotorRPM \r\n11.EGT \r\n12.BatteryVoltage \r\n13.PumpVoltage \r\n14.Fuel" +
                    "\r\n15.Roll \r\n16.RollRef \r\n17.RollTemp \r\n18.Pitch \r\n19.PitchRef \r\n20.PitchTemp \r\n21.ServoError \r\n21.RollAkim \r\n22.PitchAkim \r\n" + 
                    "veri" + s + "= [");
                KontrolPaneli.KayitYap = true;
            }
        }

        private bool CmdEstopCanExecute(object arg)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli)
                return true;

            return false;
        }
        private void CmdEstopExecute(object obj)
        {
            KontrolPaneli.EstopUI = KontrolPaneli.EstopUI ? false : true;
            string string2send = "";
            string2send += Convert.ToString(initChars[0]) + Convert.ToString(KontrolPaneli.EstopUI) + "#";
            string2send += "\n";
            KontrolPaneli.MySerialPort.Write(string2send);
        }

        private bool CmdEnterCanExecute(object arg)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli)
                return true;

            return false;
        }
        private void CmdEnterExecute(object obj)
        {
            string string2send = "";

            if (KontrolPaneli.DataChangedFlags[1])
            {
                string2send += initChars[1] + Convert.ToString(KontrolPaneli.BaslangicKutle) + "#";
                KontrolPaneli.DataChangedFlags[1] = false;
            }
            //if (KontrolPaneli.DataChangedFlags[2])
            //{
            //    string2send += initChars[2] + Convert.ToString(KontrolPaneli.Quad_roll_min_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[2] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[3])
            //{
            //    string2send += initChars[3] + Convert.ToString(KontrolPaneli.Quad_roll_max_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[3] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[4])
            //{
            //    string2send += initChars[4] + Convert.ToString(KontrolPaneli.Quad_pitch_min_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[4] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[5])
            //{
            //    string2send += initChars[5] + Convert.ToString(KontrolPaneli.Quad_pitch_max_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[5] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[6])
            //{
            //    string2send += initChars[6] + Convert.ToString(KontrolPaneli.Quad_yaw_min_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[6] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[7])
            //{
            //    string2send += initChars[7] + Convert.ToString(KontrolPaneli.Quad_yaw_max_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[7] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[8])
            //{
            //    string2send += initChars[8] + Convert.ToString(KontrolPaneli.Quad_z_min_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[8] = false;
            //}
            //if (KontrolPaneli.DataChangedFlags[9])
            //{
            //    string2send += initChars[9] + Convert.ToString(KontrolPaneli.Quad_z_max_ui) + "#";
            //    KontrolPaneli.DataChangedFlags[9] = false;
            //}
            string2send += "\n";
            KontrolPaneli.MySerialPort.Write(string2send);
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                MyStopwatch.Stop();
                KontrolPaneli.Time = MyStopwatch.ElapsedMilliseconds;
                MyStopwatch.Start();

                string receivedData = KontrolPaneli.MySerialPort.ReadLine();
                
                if (receivedData[0] == '%') // if datas are received
                {
                    KontrolPaneli.Baglanti = EnumBaglanti.Bagli;

                    receivedData = receivedData.Remove(0, 1);
                    string[] datas = receivedData.Split('#');

                    //GELEN DATALARI KontrolPaneli DEĞİŞKENLERINE ATA
                    #region
                    // BURADA DEĞİŞİKLİK YAPINCA ALTTA if (KontrolPaneli.KayitYap) İÇİNDE DEĞİŞİKLİK YAPMAYI UNUTMA
                    KontrolPaneli.BaslangicKutle.DegerFloat = float.Parse(datas[0], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.TahminiKutle = float.Parse(datas[1], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.LQR = float.Parse(datas[2], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.HoverServoAcisi = float.Parse(datas[3], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoRollRef[0] = float.Parse(datas[4], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollRef[1] = float.Parse(datas[5], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollRef[2] = float.Parse(datas[6], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollRef[3] = float.Parse(datas[7], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoPitchRef[0] = float.Parse(datas[8], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchRef[1] = float.Parse(datas[9], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchRef[2] = float.Parse(datas[10], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchRef[3] = float.Parse(datas[11], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoRollAct[0] = float.Parse(datas[12], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollAct[1] = float.Parse(datas[13], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollAct[2] = float.Parse(datas[14], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollAct[3] = float.Parse(datas[15], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoPitchAct[0] = float.Parse(datas[16], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchAct[1] = float.Parse(datas[17], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchAct[2] = float.Parse(datas[18], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchAct[3] = float.Parse(datas[19], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoRollTemp[0] = float.Parse(datas[20], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollTemp[1] = float.Parse(datas[21], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollTemp[2] = float.Parse(datas[22], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoRollTemp[3] = float.Parse(datas[23], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoPitchTemp[0] = float.Parse(datas[24], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchTemp[1] = float.Parse(datas[25], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchTemp[2] = float.Parse(datas[26], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.ServoPitchTemp[3] = float.Parse(datas[27], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.ServoBatVolt = float.Parse(datas[28], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.Durum[0] = (EnumMotorStatus)(int.Parse(datas[29]));
                    KontrolPaneli.Durum[1] = (EnumMotorStatus)(int.Parse(datas[30]));
                    KontrolPaneli.Durum[2] = (EnumMotorStatus)(int.Parse(datas[31]));
                    KontrolPaneli.Durum[3] = (EnumMotorStatus)(int.Parse(datas[32]));

                    KontrolPaneli.RPM[0] = long.Parse(datas[33], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.RPM[1] = long.Parse(datas[34], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.RPM[2] = long.Parse(datas[35], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.RPM[3] = long.Parse(datas[36], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.Itki[0] = float.Parse(datas[37], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.Itki[1] = float.Parse(datas[38], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.Itki[2] = float.Parse(datas[39], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.Itki[3] = float.Parse(datas[40], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.EGT[0] = int.Parse(datas[41], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.EGT[1] = int.Parse(datas[42], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.EGT[2] = int.Parse(datas[43], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.EGT[3] = int.Parse(datas[44], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.JetBatVolt[0] = float.Parse(datas[45], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.JetBatVolt[1] = float.Parse(datas[46], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.JetBatVolt[2] = float.Parse(datas[47], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.JetBatVolt[3] = float.Parse(datas[48], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.QuadRef[0] = float.Parse(datas[49], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.QuadRef[1] = float.Parse(datas[50], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.QuadRef[2] = float.Parse(datas[51], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.QuadRef[3] = float.Parse(datas[52], CultureInfo.InvariantCulture.NumberFormat);

                    KontrolPaneli.QuadAct[0] = float.Parse(datas[53], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.QuadAct[1] = float.Parse(datas[54], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.QuadAct[2] = float.Parse(datas[55], CultureInfo.InvariantCulture.NumberFormat);
                    KontrolPaneli.QuadAct[3] = float.Parse(datas[56], CultureInfo.InvariantCulture.NumberFormat);

                    //KontrolPaneli.Fuel = float.Parse(datas[10], CultureInfo.InvariantCulture.NumberFormat)/1000f;
                    #endregion

                    // ERRORLARI GUNCELLE
                    #region
                    KontrolPaneli.ServoRollErr[0] = KontrolPaneli.ServoRollRef[0] - KontrolPaneli.ServoRollAct[0];
                    KontrolPaneli.ServoRollErr[1] = KontrolPaneli.ServoRollRef[1] - KontrolPaneli.ServoRollAct[1];
                    KontrolPaneli.ServoRollErr[2] = KontrolPaneli.ServoRollRef[2] - KontrolPaneli.ServoRollAct[2];
                    KontrolPaneli.ServoRollErr[3] = KontrolPaneli.ServoRollRef[3] - KontrolPaneli.ServoRollAct[3];

                    KontrolPaneli.ServoPitchErr[0] = KontrolPaneli.ServoPitchRef[0] - KontrolPaneli.ServoPitchAct[0];
                    KontrolPaneli.ServoPitchErr[1] = KontrolPaneli.ServoPitchRef[1] - KontrolPaneli.ServoPitchAct[1];
                    KontrolPaneli.ServoPitchErr[2] = KontrolPaneli.ServoPitchRef[2] - KontrolPaneli.ServoPitchAct[2];
                    KontrolPaneli.ServoPitchErr[3] = KontrolPaneli.ServoPitchRef[3] - KontrolPaneli.ServoPitchAct[3];

                    KontrolPaneli.QuadErr[0] = KontrolPaneli.QuadRef[0] - KontrolPaneli.QuadAct[0];
                    KontrolPaneli.QuadErr[1] = KontrolPaneli.QuadRef[1] - KontrolPaneli.QuadAct[1];
                    KontrolPaneli.QuadErr[2] = KontrolPaneli.QuadRef[2] - KontrolPaneli.QuadAct[2];
                    KontrolPaneli.QuadErr[3] = KontrolPaneli.QuadRef[3] - KontrolPaneli.QuadAct[3];
                    #endregion

                    if (KontrolPaneli.KayitYap)
                    {
                        // GELEN DATALARI TEXT DOSYASINA KAYDET
                        #region
                        // BURADA DEĞİŞİKLİK YAPINCA CmdKaydetExecute İÇİNDE SIRALAMAYI BELİRTEN YERDE DEĞİŞİKLİK YAPMAYI UNUTMA
                        sw.Write(
                            KontrolPaneli.Time.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.BaslangicKutle.DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.TahminiKutle.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.LQR.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.HoverServoAcisi.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");

                        for (int i = 0; i < 4; i++)
                        {
                            sw.Write(KontrolPaneli.ServoRollRef[i].ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                        }
                        sw.Write(
                            KontrolPaneli.ServoRollRef[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollRef[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollRef[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollRef[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            KontrolPaneli.ServoPitchRef[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchRef[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchRef[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchRef[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            KontrolPaneli.ServoRollAct[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollAct[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollAct[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollAct[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            KontrolPaneli.ServoPitchAct[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchAct[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchAct[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchAct[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            KontrolPaneli.ServoRollTemp[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollTemp[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollTemp[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoRollTemp[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            KontrolPaneli.ServoPitchTemp[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchTemp[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchTemp[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.ServoPitchTemp[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                           
                            KontrolPaneli.ServoBatVolt.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            
                            (int)KontrolPaneli.Durum[0] + "," +
                            (int)KontrolPaneli.Durum[1] + "," +
                            (int)KontrolPaneli.Durum[2] + "," +
                            (int)KontrolPaneli.Durum[3] + "," +
                            
                            KontrolPaneli.RPM[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.RPM[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.RPM[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.RPM[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            
                            KontrolPaneli.Itki[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.Itki[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.Itki[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.Itki[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            
                            KontrolPaneli.EGT[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.EGT[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.EGT[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.EGT[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            
                            KontrolPaneli.JetBatVolt[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.JetBatVolt[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.JetBatVolt[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.JetBatVolt[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            KontrolPaneli.QuadRef[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.QuadRef[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.QuadRef[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.QuadRef[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            
                            KontrolPaneli.QuadAct[0].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.QuadAct[1].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.QuadAct[2].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            KontrolPaneli.QuadAct[3].ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            //KontrolPaneli.Quad_roll_min_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            //KontrolPaneli.Quad_roll_max_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            //KontrolPaneli.Quad_pitch_min_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            //KontrolPaneli.Quad_pitch_max_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            //KontrolPaneli.Quad_yaw_min_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            //KontrolPaneli.Quad_yaw_max_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            //KontrolPaneli.Quad_z_min_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                            //KontrolPaneli.Quad_z_max_veh.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +

                            //KontrolPaneli.Fuel.ToString(CultureInfo.InvariantCulture.NumberFormat)              + "," +
                            Environment.NewLine);
                        #endregion
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

