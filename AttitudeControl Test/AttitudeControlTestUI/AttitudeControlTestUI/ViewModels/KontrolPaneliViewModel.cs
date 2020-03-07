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

        private int sqFlags = 0;
        private int? gelenByte = null;
        private byte[] receivedData = new byte[82];
        private byte[] initSeq = { 0xAB, 0xCD, 0xEF };

        // PROTOKOL DEĞİŞTİĞİ İÇİN BUNLARI KULLANMIYORUM ARTIK
        //byte[] initChars = { (byte)'"', (byte)'!', (byte)'\'', (byte)'^', (byte)'+', (byte)'%', (byte)'&', (byte)'/', (byte)'(', (byte)')' };
        //byte frameEnd = (byte)'#';

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
                sw.Write("veri" + s + "= [");
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
            // ABDULLAH TODO
            // ÖNCE BAŞINA SONUNA KARAKTER EKLEDİĞİMİZ VERSİYONU İMPLEMENTE ETMİŞTİM
            // PROTOKOL DEĞİŞTİĞİ İÇİN AŞAĞIDAKİLER KULLANILMIYOR.
            // SEN AYARLICAN ARTIK

            CmdEnterExecute(null);

            //string string2send = "";
            //string2send += Convert.ToString(initChars[0]) + Convert.ToString(KontrolPaneli.EstopUI) + "#";
            //string2send += "\n";
            //KontrolPaneli.MySerialPort.Write(string2send);
        }

        private bool CmdEnterCanExecute(object arg)
        {
            if (KontrolPaneli.Baglanti >= EnumBaglanti.Bagli)
                return true;

            return false;
        }
        private void CmdEnterExecute(object obj)
        {
            // add initial sequence
            List<byte> bytes2send = new List<byte> { initSeq[0], initSeq[1], initSeq[2] };

            // ÖNCE BAŞINA SONUNA KARAKTER EKLEDİĞİMİZ VERSİYONU İMPLEMENTE ETMİŞTİM
            // PROTOKOL DEĞİŞTİĞİ İÇİN AŞAĞIDAKİLER KULLANILMIYOR.
            // SEN AYARLICAN ARTIK
            //if (KontrolPaneli.DataChangedFlags[1])
            //{
            //    bytes2send.Add(initChars[1]);
            //    bytes2send.Add(KontrolPaneli.BaslangicKutle.DegerByte);
            //    bytes2send.Add(frameEnd);
            //    KontrolPaneli.DataChangedFlags[1] = false;
            //}
            //for (int i = 0; i < 4; i++)
            //{
            //    if (KontrolPaneli.DataChangedFlags[i+2])
            //    {
            //        bytes2send.Add(initChars[i + 2]);
            //        bytes2send.Add(KontrolPaneli.QuadMinUI[i].DegerByte);
            //        bytes2send.Add(frameEnd);
            //        KontrolPaneli.DataChangedFlags[i+2] = false;
            //    }
            //}
            //for (int i = 0; i < 4; i++)
            //{
            //    if (KontrolPaneli.DataChangedFlags[i + 6])
            //    {
            //        bytes2send.Add(initChars[i + 6]);
            //        bytes2send.Add(KontrolPaneli.QuadMaxUI[i].DegerByte);
            //        bytes2send.Add(frameEnd);
            //        KontrolPaneli.DataChangedFlags[i + 6] = false;
            //    }
            //}

            ushort checksum = 0;
            bytes2send.Add(Convert.ToByte(KontrolPaneli.EstopUI));
            bytes2send.Add(KontrolPaneli.BaslangicKutle.DegerByte);
            for (int i = 0; i < 4; i++)
            {
                bytes2send.Add(KontrolPaneli.QuadMinUI[i].DegerByte);
            }
            for (int i = 0; i < 4; i++)
            {
                bytes2send.Add(KontrolPaneli.QuadMaxUI[i].DegerByte);
            }
            // calculating checksum
            for (int i = 0; i < bytes2send.Count; i++)
            {
                checksum += bytes2send[i];
            }
            List<byte> a = BitConverter.GetBytes(checksum).ToList();
            bytes2send.AddRange(a);
            KontrolPaneli.MySerialPort.Write(bytes2send.ToArray(),0,bytes2send.Count);
        } //end of cmdEnterExecute

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                MyStopwatch.Stop();
                KontrolPaneli.Time = MyStopwatch.ElapsedMilliseconds;
                MyStopwatch.Start();

                KontrolPaneli.Baglanti = EnumBaglanti.Bagli;

                // checking for initial sequence
                while (sqFlags != 3 && KontrolPaneli.MySerialPort.BytesToRead >= 3)
                {
                    //if (gelenByte == null)
                    //{
                    //    gelenByte = KontrolPaneli.MySerialPort.ReadByte();
                    //}
                    gelenByte = KontrolPaneli.MySerialPort.ReadByte();

                    if (gelenByte == 0xAB)
                    {
                        sqFlags++;
                        gelenByte = KontrolPaneli.MySerialPort.ReadByte();
                        if (gelenByte == 0xCD)
                        {
                            sqFlags++;
                            gelenByte = KontrolPaneli.MySerialPort.ReadByte();
                            if (gelenByte == 0xEF)
                            {
                                sqFlags++;
                                break;
                            }
                            sqFlags = 0;
                            continue;
                        }
                        sqFlags = 0;
                        continue;
                    }
                    //gelenByte = null;
                    continue;
                }
                
                if (sqFlags == 3) // if initial squence is received
                {
                    sqFlags = 0;
                    KontrolPaneli.MySerialPort.Read(receivedData, 0, 79);
                    ushort sum = (ushort)(initSeq[0] + initSeq[1] + initSeq[2]);
                    for (int i = 0; i < 77; i++)
                    {
                        sum += receivedData[i];
                    }
                    ushort receivedChecksum = receivedData[77];
                    receivedChecksum = (ushort)((receivedData[78] << 8) | receivedChecksum);

                    if (sum == receivedChecksum)
                    {
                        //receivedData = receivedData.Remove(0, 1);
                        //string[] datas = receivedData.Split('#');

                        //ABDULLAH TODO
                        //GELEN DATALARI KontrolPaneli DEĞİŞKENLERINE ATA
                        #region
                        // BURADA DEĞİŞİKLİK YAPINCA ALTTA if (KontrolPaneli.KayitYap) İÇİNDE DEĞİŞİKLİK YAPMAYI UNUTMA
                        for (int i = 0; i < 4; i++)
                        {
                            KontrolPaneli.Durum[i] = (EnumMotorStatus)receivedData[0 + i * 6];
                            KontrolPaneli.RPM[i].DegerUshort = (ushort)(receivedData[2 + i * 6] << 8 | receivedData[1 + i * 6]);
                            KontrolPaneli.Itki[i].DegerByte = receivedData[3 + i * 6];
                            KontrolPaneli.EGT[i].DegerByte = receivedData[4 + i * 6];
                            KontrolPaneli.JetBatVolt[i].DegerByte = receivedData[5 + i * 6];
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            KontrolPaneli.ServoRollRef[i].DegerByte = receivedData[24 + i * 6];
                            KontrolPaneli.ServoRollAct[i].DegerByte = receivedData[25 + i * 6];
                            KontrolPaneli.ServoRollTemp[i].DegerByte = receivedData[26 + i * 6];
                            KontrolPaneli.ServoPitchRef[i].DegerByte = receivedData[27 + i * 6];
                            KontrolPaneli.ServoPitchAct[i].DegerByte = receivedData[28 + i * 6];
                            KontrolPaneli.ServoPitchTemp[i].DegerByte= receivedData[29 + i * 6];
                        }

                        KontrolPaneli.ServoBatVolt.DegerByte = receivedData[48];

                        for (int i = 0; i < 4; i++)
                        {
                            KontrolPaneli.QuadRef[i].DegerByte = receivedData[49 + i * 4];
                            KontrolPaneli.QuadAct[i].DegerByte = receivedData[50 + i * 4];
                            KontrolPaneli.QuadMinVeh[i].DegerByte = receivedData[51 + i * 4];
                            KontrolPaneli.QuadMaxVeh[i].DegerByte = receivedData[52 + i * 4];
                        }

                        KontrolPaneli.EstopVeh = Convert.ToBoolean(receivedData[65]);
                        KontrolPaneli.TahminiKutle.DegerByte = receivedData[66];
                        KontrolPaneli.LQR = Convert.ToBoolean(receivedData[67]);
                        KontrolPaneli.HoverServoAcisi.DegerByte = receivedData[68];

                        KontrolPaneli.RcRoll.DegerByte = receivedData[69];
                        KontrolPaneli.RcPitch.DegerByte = receivedData[70];
                        KontrolPaneli.RcYaw.DegerByte = receivedData[71];
                        KontrolPaneli.RcZ.DegerByte = receivedData[72];
                        KontrolPaneli.RcThrust.DegerByte = receivedData[73];
                        KontrolPaneli.RcServo.DegerByte = receivedData[74];
                        KontrolPaneli.RcLQR.DegerByte = receivedData[75];
                        KontrolPaneli.RcEstop.DegerByte = receivedData[76];

                        //KontrolPaneli.Fuel = float.Parse(datas[10], CultureInfo.InvariantCulture.NumberFormat)/1000f;
                        #endregion

                        // ERRORLARI GUNCELLE
                        #region
                        KontrolPaneli.ServoRollErr[0] = KontrolPaneli.ServoRollRef[0].DegerFloat - KontrolPaneli.ServoRollAct[0].DegerFloat;
                        KontrolPaneli.ServoRollErr[1] = KontrolPaneli.ServoRollRef[1].DegerFloat - KontrolPaneli.ServoRollAct[1].DegerFloat;
                        KontrolPaneli.ServoRollErr[2] = KontrolPaneli.ServoRollRef[2].DegerFloat - KontrolPaneli.ServoRollAct[2].DegerFloat;
                        KontrolPaneli.ServoRollErr[3] = KontrolPaneli.ServoRollRef[3].DegerFloat - KontrolPaneli.ServoRollAct[3].DegerFloat;

                        KontrolPaneli.ServoPitchErr[0] = KontrolPaneli.ServoPitchRef[0].DegerFloat - KontrolPaneli.ServoPitchAct[0].DegerFloat;
                        KontrolPaneli.ServoPitchErr[1] = KontrolPaneli.ServoPitchRef[1].DegerFloat - KontrolPaneli.ServoPitchAct[1].DegerFloat;
                        KontrolPaneli.ServoPitchErr[2] = KontrolPaneli.ServoPitchRef[2].DegerFloat - KontrolPaneli.ServoPitchAct[2].DegerFloat;
                        KontrolPaneli.ServoPitchErr[3] = KontrolPaneli.ServoPitchRef[3].DegerFloat - KontrolPaneli.ServoPitchAct[3].DegerFloat;

                        KontrolPaneli.QuadErr[0] = KontrolPaneli.QuadRef[0].DegerFloat - KontrolPaneli.QuadAct[0].DegerFloat;
                        KontrolPaneli.QuadErr[1] = KontrolPaneli.QuadRef[1].DegerFloat - KontrolPaneli.QuadAct[1].DegerFloat;
                        KontrolPaneli.QuadErr[2] = KontrolPaneli.QuadRef[2].DegerFloat - KontrolPaneli.QuadAct[2].DegerFloat;
                        KontrolPaneli.QuadErr[3] = KontrolPaneli.QuadRef[3].DegerFloat - KontrolPaneli.QuadAct[3].DegerFloat;
                        KontrolPaneli.Baglanti = EnumBaglanti.Bagli;
                        #endregion

                        if (KontrolPaneli.KayitYap)
                        {
                            // ABDULLAH TODO
                            // BURAYI DÜZENLEMEN LAZIM
                            // GELEN DATALARI TEXT DOSYASINA KAYDET
                            #region
                            // BURADA DEĞİŞİKLİK YAPINCA CmdKaydetExecute İÇİNDE SIRALAMAYI BELİRTEN YERDE DEĞİŞİKLİK YAPMAYI UNUTMA
                            sw.Write(
                                KontrolPaneli.Time.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.BaslangicKutle.DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.TahminiKutle.DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.LQR.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," +
                                KontrolPaneli.HoverServoAcisi.DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",",
                                KontrolPaneli.ServoBatVolt.DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",",
                                KontrolPaneli.Fuel.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");

                            for (int i = 0; i < 4; i++)
                            {
                                sw.Write(KontrolPaneli.ServoRollRef[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.ServoRollRef[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.ServoPitchRef[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.ServoRollAct[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.ServoPitchAct[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.ServoRollTemp[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.ServoPitchTemp[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write((int)KontrolPaneli.Durum[i] + ",");
                                sw.Write(KontrolPaneli.RPM[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.Itki[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.EGT[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.JetBatVolt[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadRef[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadAct[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadMinUI[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadMaxUI[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadMinVeh[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadMaxVeh[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                                sw.Write(KontrolPaneli.QuadMaxVeh[i].DegerFloat.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",");
                            }
                            sw.Write(Environment.NewLine);
                            #endregion
                        }
                    }
                    else
                    {
                        // ABDULLAH TODO
                        // CHECKSUM'DA ERROR VARSA NE YAPILACAĞI BURADA İMPLEMENTE EDİLECEK
                        // TODO checksum ERROR !!!!!!!!!!!!!!!!!!!
                        KontrolPaneli.Baglanti = EnumBaglanti.HataliVeri;
                    }
                }
                //else
                //{
                //    KontrolPaneli.MySerialPort.DiscardInBuffer();
                //    KontrolPaneli.Baglanti = EnumBaglanti.HataliVeri;
                //}
            }
            catch (Exception)
            {
                MessageBox.Show("data okuma işlemi yarım kaldı");
            }
        } //end of serialPort_DataReceived

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

