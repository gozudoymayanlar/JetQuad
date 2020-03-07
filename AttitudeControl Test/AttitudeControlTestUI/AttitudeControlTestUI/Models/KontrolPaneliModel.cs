using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Timers;
using LiveCharts;
using LiveCharts.Wpf;
using System.Globalization;
using System.Drawing;
using System.Collections.Specialized;

namespace AttitudeControlTestUI.Models
{
    class KontrolPaneliModel : BaseViewModel
    {
        #region PRIVATE FIELDS
        private ObservableCollection<string> _comPortCollection = new ObservableCollection<string> { };
        private SerialPort _mySerialPort;
        private System.Timers.Timer _timer;
        private string _seciliComPort;
        private string _baudRate = "115200";
        private bool _kayitYap = false;
        private EnumBaglanti _baglanti = EnumBaglanti.BaglantiYok;
        private bool _estopUI = false;
        private bool _estopVeh = false;

        // tekli değişkenler - sol taraftaki
        private MyVeri _baslangicKutle = new MyVeri(51.4f, 0, 70);
        private MyVeri _tahminiKutle = new MyVeri(50.0f, 0, 70);
        private bool _lqr = false;
        private MyVeri _hoverServoAcisi = new MyVeri(20, 0, 45);
        //private int _motorThrottle = 52;


        // servo motor değişkenleri sağ üst geniş tablo
        private ObservableCollection<MyVeri> _servoRollRef = new ObservableCollection<MyVeri>
            { new MyVeri(0, -30, 30), new MyVeri(1, -30, 30), new MyVeri(2, -30, 30), new MyVeri(3, -30, 30) };

        private ObservableCollection<MyVeri> _servoRollAct = new ObservableCollection<MyVeri>
            { new MyVeri(4, -30, 30), new MyVeri(5, -30, 30), new MyVeri(6, -30, 30), new MyVeri(7, -30, 30) };

        private ObservableCollection<float> _servoRollErr = new ObservableCollection<float> { 4,5,6,7 };

        private ObservableCollection<MyVeri> _servoRollTemp = new ObservableCollection<MyVeri>
            { new MyVeri(12, 10, 60), new MyVeri(13, 10, 60), new MyVeri(14, 10, 60), new MyVeri(15, 10, 60) };


        private ObservableCollection<MyVeri> _servoPitchRef = new ObservableCollection<MyVeri>
            { new MyVeri(0, -30, 30), new MyVeri(0, -30, 30), new MyVeri(0, -30, 30), new MyVeri(0, -30, 30) };

        private ObservableCollection<MyVeri> _servoPitchAct = new ObservableCollection<MyVeri>
            { new MyVeri(0, -30, 30), new MyVeri(0, -30, 30), new MyVeri(0, -30, 30), new MyVeri(0, -30, 30) };

        private ObservableCollection<float> _servoPitchErr = new ObservableCollection<float> { 0,1,2,3 };

        private ObservableCollection<MyVeri> _servoPitchTemp = new ObservableCollection<MyVeri>
            { new MyVeri(15, 10, 60), new MyVeri(30, 10, 60), new MyVeri(45, 10, 60), new MyVeri(60, 10, 60) };


        // servo batarya için 16.8 - 12
        private MyVeri _servoBatVolt = new MyVeri(16.8f, 12f, 16.8f);

        // jet motor değişkenler - sağ alt sol tablo
        private ObservableCollection<EnumMotorStatus> _durum = new ObservableCollection<EnumMotorStatus> { EnumMotorStatus.BaglantiYok, EnumMotorStatus.AccelDly, EnumMotorStatus.HataliVeri, EnumMotorStatus.Run };

        private ObservableCollection<MyVeri2Byte> _rpm = new ObservableCollection<MyVeri2Byte>
            { new MyVeri2Byte(30000, 0, 130000), new MyVeri2Byte(75000, 0, 130000), new MyVeri2Byte(100000, 0, 130000), new MyVeri2Byte(120000, 0, 130000) };

        private ObservableCollection<MyVeri> _itki = new ObservableCollection<MyVeri>
            { new MyVeri(32f, 0, 200), new MyVeri(33f, 0, 200), new MyVeri(33f, 0, 200), new MyVeri(34f, 0, 200) };

        private ObservableCollection<MyVeri> _egt = new ObservableCollection<MyVeri>
            { new MyVeri(0, -10, 1000), new MyVeri(300, -10, 1000), new MyVeri(600, -10, 1000), new MyVeri(1400, -10, 1000) };
        
        // jet bataryalar için 12.6 - 9
        private ObservableCollection<MyVeri> _jetBatVolt = new ObservableCollection<MyVeri>
            {  new MyVeri(12.6f, 0, 20), new MyVeri(11.4f, 0, 20), new MyVeri(10.4f, 0, 20), new MyVeri(9f, 0, 20)};

        // Jet quad değişkenleri tablosu - sağ alt sağ tablo
        private ObservableCollection<MyVeri> _quadRef = new ObservableCollection<MyVeri> { new MyVeri(4, -45, 45), new MyVeri(5, -45, 45), new MyVeri(6, -45, 45), new MyVeri(7, -45, 45) };
        private ObservableCollection<MyVeri> _quadAct = new ObservableCollection<MyVeri> { new MyVeri(8, -45, 45), new MyVeri(9, -45, 45), new MyVeri(10, -45, 45), new MyVeri(1, -45, 45) };
        private ObservableCollection<float> _quadErr = new ObservableCollection<float> { 12,13,14,15 };

        //private float _quad_roll_min_ui = 61;
        //private float _quad_roll_max_ui = 62;
        //private float _quad_pitch_min_ui = 63;
        //private float _quad_pitch_max_ui = 64;
        //private float _quad_yaw_min_ui = 65;
        //private float _quad_yaw_max_ui = 66;
        //private float _quad_z_min_ui = 67;
        //private float _quad_z_max_ui = 68;

        //private float _quad_roll_min_veh = 61;
        //private float _quad_roll_max_veh = 62;
        //private float _quad_pitch_min_veh = 63;
        //private float _quad_pitch_max_veh = 64;
        //private float _quad_yaw_min_veh = 65;
        //private float _quad_yaw_max_veh = 66;
        //private float _quad_z_min_veh = 67;
        //private float _quad_z_max_veh = 68;

        private ObservableCollection<MyVeri> _quadMinUI = new ObservableCollection<MyVeri>
            { new MyVeri(61, -45, 45, 0), new MyVeri(62, -45, 45, 1), new MyVeri(63, 0, 360, 2), new MyVeri(64, 0, 10, 3) };

        private ObservableCollection<MyVeri> _quadMaxUI = new ObservableCollection<MyVeri>
            { new MyVeri(65, -45, 45, 0), new MyVeri(66, -45, 45, 1), new MyVeri(67, 0, 360, 2), new MyVeri(68, 0, 10, 3) };

        private ObservableCollection<MyVeri> _quadMinVeh = new ObservableCollection<MyVeri>
            { new MyVeri(69, -45, 45, 0), new MyVeri(70, -45, 45, 1), new MyVeri(71, 0, 360, 2), new MyVeri(72, 0, 10, 3) };

        private ObservableCollection<MyVeri> _quadMaxVeh = new ObservableCollection<MyVeri>
            { new MyVeri(73, -45, 45, 0), new MyVeri(74, -45, 45, 1), new MyVeri(75, 0, 360, 2), new MyVeri(76, 0, 10, 3) };

        private MyVeri _rcRoll = new MyVeri(500, 500, 2500);
        private MyVeri _rcPitch = new MyVeri(1000, 500, 2500);
        private MyVeri _rcYaw = new MyVeri(1300, 500, 2500);
        private MyVeri _rcZ = new MyVeri(1500, 500, 2500);
        private MyVeri _rcThrust = new MyVeri(1700, 500, 2500);
        private MyVeri _rcServo = new MyVeri(2000, 500, 2500);
        private MyVeri _rcLQR = new MyVeri(2200, 500, 2500);
        private MyVeri _rcEstop = new MyVeri(2500, 500, 2500);

        private float _fuel = 30f; // ???????????????

        private long _time = 0;

        private DateTime _startingDate;

        private bool[] _dataChangedFlags = { false, false, false, false, false, false, false, false, false, false };

        //private float _pumpVoltage = 15.4f;

        //private long _referenceRPM = 6000;
        //private float _motorRPM_gauge = 6.2f;

        //private int _servoError;

        //private SeriesCollection _rollSeriesCollection;
        //private List<DateTimePoint> _rollBuffer;
        //private List<DateTimePoint> _rollRefBuffer;

        //private SeriesCollection _pitchSeriesCollection;
        //private List<DateTimePoint> _pitchBuffer;
        //private List<DateTimePoint> _pitchRefBuffer;
        //private float _currentPitch;
        //private float _currentPitchRef;

        //private float _pitchTemp;
        //private float _rollAkim;
        //private float _pitchAkim;

        //private int _grafikteGosterilecekSaniye = 3;
        #endregion

        public KontrolPaneliModel()
        {
            StartingDate = new DateTime();
            Baudrate = "115200";
            BaslangicKutle.PropertyChanged += (s, e) => 
            {
                _dataChangedFlags[1] = true;
            };
            foreach (var item in QuadMinUI)
            {
                item.PropertyChanged += (s, e) =>
                {
                    _dataChangedFlags[2 + item.Idx] = true;
                };
            }
            foreach (var item in QuadMaxUI)
            {
                item.PropertyChanged += (s, e) =>
                {
                    _dataChangedFlags[6 + item.Idx] = true;
                };
            }
        }

        #region PUBLIC PROPERTIES
        public ObservableCollection<string> ComPortCollection
        {
            get { return _comPortCollection; }
            set {_comPortCollection = value; OnPropertyChanged(nameof(ComPortCollection)); }
        }
        public SerialPort MySerialPort
        {
            get { return _mySerialPort; }
            set { _mySerialPort = value; OnPropertyChanged(nameof(MySerialPort)); }
        }
        public Timer Timer
        {
            get { return _timer; }
            set { _timer = value; OnPropertyChanged(nameof(Timer)); }
        }
        public string SeciliComPort
        {
            get { return _seciliComPort; }
            set { _seciliComPort = value; OnPropertyChanged(nameof(SeciliComPort)); }
        }
        public string Baudrate
        {
            get { return _baudRate; }
            set { _baudRate = value; OnPropertyChanged(nameof(Baudrate)); }
        }
        public bool KayitYap
        {
            get { return _kayitYap; }
            set { _kayitYap = value; OnPropertyChanged(nameof(KayitYap)); }
        }
        public EnumBaglanti Baglanti
        {
            get { return _baglanti; }
            set
            {
                _baglanti = value; OnPropertyChanged(nameof(Baglanti));
                if (value == EnumBaglanti.BaglantiYok)
                {
                    Durum[0] = EnumMotorStatus.BaglantiYok;
                    Durum[1] = EnumMotorStatus.BaglantiYok;
                    Durum[2] = EnumMotorStatus.BaglantiYok;
                    Durum[3] = EnumMotorStatus.BaglantiYok;
                }
            }
        }
        
        public MyVeri BaslangicKutle
        {
            get { return _baslangicKutle; }
            set {
                _baslangicKutle = value;
                OnPropertyChanged(nameof(BaslangicKutle));
                _dataChangedFlags[1] = true;
            }
        }
        public MyVeri TahminiKutle
        {
            get { return _tahminiKutle; }
            set { _tahminiKutle = value; OnPropertyChanged(nameof(TahminiKutle)); }
        }
        public bool LQR
        {
            get { return _lqr; }
            set { _lqr = value; OnPropertyChanged(nameof(LQR)); }
        }
        public MyVeri HoverServoAcisi
        {
            get { return _hoverServoAcisi; }
            set { _hoverServoAcisi = value; OnPropertyChanged(nameof(HoverServoAcisi)); }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ObservableCollection<MyVeri> ServoRollRef
        {
            get { return _servoRollRef; }
            set { _servoRollRef = value; OnPropertyChanged(nameof(ServoRollRef)); }
        }
        public ObservableCollection<MyVeri> ServoRollAct
        {
            get { return _servoRollAct; }
            set
            {
                _servoRollAct = value;
                OnPropertyChanged(nameof(ServoRollAct));
            }
        }
        public ObservableCollection<float> ServoRollErr
        {
            get { return _servoRollErr; }
            set { _servoRollErr = value; OnPropertyChanged(nameof(ServoRollErr)); }
        }
        public ObservableCollection<MyVeri> ServoRollTemp
        {
            get { return _servoRollTemp; }
            set { _servoRollTemp = value; OnPropertyChanged(nameof(ServoRollTemp)); }
        }

        public ObservableCollection<MyVeri> ServoPitchRef
        {
            get { return _servoPitchRef; }
            set { _servoPitchRef = value; OnPropertyChanged(nameof(ServoPitchRef)); }
        }
        public ObservableCollection<MyVeri> ServoPitchAct
        {
            get { return _servoPitchAct; }
            set
            {
                _servoPitchAct = value;
                OnPropertyChanged(nameof(ServoPitchAct));
            }
        }
        public ObservableCollection<float> ServoPitchErr
        {
            get { return _servoPitchErr; }
            set { _servoPitchErr = value; OnPropertyChanged(nameof(ServoPitchErr)); }
        }
        public ObservableCollection<MyVeri> ServoPitchTemp
        {
            get { return _servoPitchTemp; }
            set { _servoPitchTemp = value; OnPropertyChanged(nameof(ServoPitchTemp)); }
        }
        public MyVeri ServoBatVolt
        {
            get { return _servoBatVolt; }
            set { _servoBatVolt = value; OnPropertyChanged(nameof(ServoBatVolt)); }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ObservableCollection<EnumMotorStatus> Durum
        {
            get { return _durum; }
            set { _durum = value; OnPropertyChanged(nameof(Durum)); }
        }
        public ObservableCollection<MyVeri2Byte> RPM
        {
            get { return _rpm; }
            set { _rpm = value; OnPropertyChanged(nameof(RPM)); }
        }
        public ObservableCollection<MyVeri> Itki
        {
            get { return _itki; }
            set { _itki = value; OnPropertyChanged(nameof(Itki)); }
        }
        public ObservableCollection<MyVeri> EGT
        {
            get { return _egt; }
            set { _egt = value; OnPropertyChanged(nameof(EGT)); }
        }
        public ObservableCollection<MyVeri> JetBatVolt
        {
            get { return _jetBatVolt; }
            set { _jetBatVolt = value; OnPropertyChanged(nameof(JetBatVolt)); }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ObservableCollection<MyVeri> QuadRef
        {
            get { return _quadRef; }
            set { _quadRef = value; OnPropertyChanged(nameof(QuadRef)); }
        }
        public ObservableCollection<MyVeri> QuadAct
        {
            get { return _quadAct; }
            set { _quadAct = value; OnPropertyChanged(nameof(QuadAct)); }
        }
        public ObservableCollection<float> QuadErr
        {
            get { return _quadErr; }
            set { _quadErr = value; OnPropertyChanged(nameof(QuadErr)); }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public long Time
        {
            get { return _time; }
            set { _time = value; OnPropertyChanged(nameof(Time)); }
        }

        public DateTime StartingDate
        {
            get { return _startingDate; }
            set { _startingDate = value; OnPropertyChanged(nameof(StartingDate)); }
        }

        public float Fuel
        {
            get { return _fuel; }
            set { _fuel = value; OnPropertyChanged(nameof(Fuel)); }
        }

        public bool[] DataChangedFlags
        {
            get { return _dataChangedFlags; }
            set { _dataChangedFlags = value; OnPropertyChanged(nameof(DataChangedFlags)); }
        }

        public bool EstopUI
        {
            get { return _estopUI; }
            set {
                _estopUI = value;
                OnPropertyChanged(nameof(EstopUI));
                _dataChangedFlags[0] = true;
            }
        }

        public bool EstopVeh
        {
            get { return _estopVeh; }
            set { _estopVeh = value; OnPropertyChanged(nameof(EstopVeh)); }
        }

        #region tekli veriler
        //public float Quad_roll_min_ui
        //{
        //    get
        //    {
        //        return _quad_roll_min_ui;
        //    }

        //    set
        //    {
        //        _quad_roll_min_ui = value;
        //        OnPropertyChanged(nameof(Quad_roll_min_ui));
        //        _dataChangedFlags[2] = true;
        //    }
        //}
        //public float Quad_roll_max_ui
        //{
        //    get
        //    {
        //        return _quad_roll_max_ui;
        //    }

        //    set
        //    {
        //        _quad_roll_max_ui = value;
        //        OnPropertyChanged(nameof(Quad_roll_max_ui));
        //        _dataChangedFlags[3] = true;
        //    }
        //}
        //public float Quad_pitch_min_ui
        //{
        //    get
        //    {
        //        return _quad_pitch_min_ui;
        //    }

        //    set
        //    {
        //        _quad_pitch_min_ui = value;
        //        OnPropertyChanged(nameof(Quad_pitch_min_ui));
        //        _dataChangedFlags[4] = true;
        //    }
        //}

        //public float Quad_pitch_max_ui
        //{
        //    get
        //    {
        //        return _quad_pitch_max_ui;
        //    }

        //    set
        //    {
        //        _quad_pitch_max_ui = value;
        //        OnPropertyChanged(nameof(Quad_pitch_max_ui));
        //        _dataChangedFlags[5] = true;
        //    }
        //}
        //public float Quad_yaw_min_ui
        //{
        //    get
        //    {
        //        return _quad_yaw_min_ui;
        //    }

        //    set
        //    {
        //        _quad_yaw_min_ui = value;
        //        OnPropertyChanged(nameof(Quad_yaw_min_ui));
        //        _dataChangedFlags[6] = true;
        //    }
        //}
        //public float Quad_yaw_max_ui
        //{
        //    get
        //    {
        //        return _quad_yaw_max_ui;
        //    }

        //    set
        //    {
        //        _quad_yaw_max_ui = value;
        //        OnPropertyChanged(nameof(Quad_yaw_max_ui));
        //        _dataChangedFlags[7] = true;
        //    }
        //}
        //public float Quad_z_min_ui
        //{
        //    get
        //    {
        //        return _quad_z_min_ui;
        //    }

        //    set
        //    {
        //        _quad_z_min_ui = value;
        //        OnPropertyChanged(nameof(Quad_z_min_ui));
        //        _dataChangedFlags[8] = true;
        //    }
        //}
        //public float Quad_z_max_ui
        //{
        //    get
        //    {
        //        return _quad_z_max_ui;
        //    }

        //    set
        //    {
        //        _quad_z_max_ui = value;
        //        OnPropertyChanged(nameof(Quad_z_max_ui));
        //        _dataChangedFlags[9] = true;
        //    }
        //}

        //public float Quad_roll_min_veh
        //{
        //    get
        //    {
        //        return _quad_roll_min_veh;
        //    }

        //    set
        //    {
        //        _quad_roll_min_veh = value;
        //        OnPropertyChanged(nameof(Quad_roll_min_veh));
        //    }
        //}
        //public float Quad_roll_max_veh
        //{
        //    get
        //    {
        //        return _quad_roll_max_veh;
        //    }

        //    set
        //    {
        //        _quad_roll_max_veh = value;
        //        OnPropertyChanged(nameof(Quad_roll_max_veh));
        //    }
        //}
        //public float Quad_pitch_min_veh
        //{
        //    get
        //    {
        //        return _quad_pitch_min_veh;
        //    }

        //    set
        //    {
        //        _quad_pitch_min_veh = value;
        //        OnPropertyChanged(nameof(Quad_pitch_min_veh));
        //    }
        //}
        //public float Quad_pitch_max_veh
        //{
        //    get
        //    {
        //        return _quad_pitch_max_veh;
        //    }

        //    set
        //    {
        //        _quad_pitch_max_veh = value;
        //        OnPropertyChanged(nameof(Quad_pitch_max_veh));
        //    }
        //}
        //public float Quad_yaw_min_veh
        //{
        //    get
        //    {
        //        return _quad_yaw_min_veh;
        //    }

        //    set
        //    {
        //        _quad_yaw_min_veh = value;
        //        OnPropertyChanged(nameof(Quad_yaw_min_veh));
        //    }
        //}
        //public float Quad_yaw_max_veh
        //{
        //    get
        //    {
        //        return _quad_yaw_max_veh;
        //    }

        //    set
        //    {
        //        _quad_yaw_max_veh = value;
        //        OnPropertyChanged(nameof(Quad_yaw_max_veh));
        //    }
        //}
        //public float Quad_z_min_veh
        //{
        //    get
        //    {
        //        return _quad_z_min_veh;
        //    }

        //    set
        //    {
        //        _quad_z_min_veh = value;
        //        OnPropertyChanged(nameof(Quad_z_min_veh));
        //    }
        //}
        //public float Quad_z_max_veh
        //{
        //    get
        //    {
        //        return _quad_z_max_veh;
        //    }

        //    set
        //    {
        //        _quad_z_max_veh = value;
        //        OnPropertyChanged(nameof(Quad_z_max_veh));
        //    }
        //}
        #endregion

        public ObservableCollection<MyVeri> QuadMinUI
        {
            get { return _quadMinUI; }
            set { _quadMinUI = value; OnPropertyChanged(nameof(QuadMinUI)); }
        }

        public ObservableCollection<MyVeri> QuadMaxUI
        {
            get { return _quadMaxUI; }
            set { _quadMaxUI = value; OnPropertyChanged(nameof(QuadMaxUI)); }
        }

        public ObservableCollection<MyVeri> QuadMinVeh
        {
            get { return _quadMinVeh; }
            set { _quadMinVeh = value; OnPropertyChanged(nameof(QuadMinVeh)); }
        }

        public ObservableCollection<MyVeri> QuadMaxVeh
        {
            get { return _quadMaxVeh; }
            set { _quadMaxVeh = value; OnPropertyChanged(nameof(QuadMaxVeh)); }
        }

        public MyVeri RcRoll
        {
            get { return _rcRoll; }
            set { _rcRoll = value; OnPropertyChanged(nameof(RcRoll)); }
        }
        public MyVeri RcPitch
        {
            get { return _rcPitch; }
            set { _rcPitch = value; OnPropertyChanged(nameof(RcPitch)); }
        }
        public MyVeri RcYaw
        {
            get { return _rcYaw; }
            set { _rcYaw = value; OnPropertyChanged(nameof(RcYaw)); }
        }
        public MyVeri RcZ
        {
            get { return _rcZ; }
            set { _rcZ = value; OnPropertyChanged(nameof(RcZ)); }
        }
        public MyVeri RcThrust
        {
            get { return _rcThrust; }
            set { _rcThrust = value; OnPropertyChanged(nameof(RcThrust)); }
        }
        public MyVeri RcServo
        {
            get { return _rcServo; }
            set { _rcServo = value; OnPropertyChanged(nameof(RcServo)); }
        }
        public MyVeri RcLQR
        {
            get { return _rcLQR; }
            set { _rcLQR = value; OnPropertyChanged(nameof(RcLQR)); }
        }
        public MyVeri RcEstop
        {
            get { return _rcEstop; }
            set { _rcEstop = value; OnPropertyChanged(nameof(RcEstop)); }
        }

        /// <summary>
        /// Ardunun Jet motor ECU'sundan okuduğu throttle değeri. Bu değer ardudan okunuyor.
        /// </summary>
        //public int MotorThrottle
        //{
        //    get { return _motorThrottle; }
        //    set { _motorThrottle = value; OnPropertyChanged(nameof(MotorThrottle)); }
        //}

        //public long ReferenceRPM
        //{
        //    get { return _referenceRPM; }
        //    set { _referenceRPM = value; OnPropertyChanged(nameof(ReferenceRPM)); }
        //}

        //public float CurrentPitch
        //{
        //    get { return _currentPitch; }
        //    set
        //    {
        //        _currentPitch = value;
        //        OnPropertyChanged(nameof(CurrentPitch));
        //        //PitchBuffer.Add(new DateTimePoint(StartingDate.AddMilliseconds(Time), value));
        //    }
        //}

        //public float CurrentPitchRef
        //{
        //    get { return _currentPitchRef; }
        //    set
        //    {
        //        _currentPitchRef = value;
        //        OnPropertyChanged(nameof(CurrentPitchRef));
        //        //PitchRefBuffer.Add(new DateTimePoint(StartingDate.AddMilliseconds(Time), value));
        //        //Update();
        //    }
        //}

        //public int ServoError
        //{
        //    get { return _servoError; }
        //    set { _servoError = value; OnPropertyChanged(nameof(ServoError)); }
        //}

        //public float RollAkim
        //{
        //    get { return _rollAkim; }
        //    set { _rollAkim = value; OnPropertyChanged(nameof(RollAkim)); }
        //}

        //public float PitchAkim
        //{
        //    get { return _pitchAkim; }
        //    set { _pitchAkim = value; OnPropertyChanged(nameof(PitchAkim)); }
        //}
        #endregion

        #region METHODS
        #endregion
    }

    enum EnumMotorStatus
    {
        BaglantiYok = -1,
        Off,
        Standby_Start,
        Ignite,
        Accelerate,
        Stabilise,
        NotUsed5,
        LearnLO,
        NotUsed7,
        Slow_Down,
        NotUsed9,
        Auto_Off,
        Run,
        AccelDly,
        Speed_Reg,
        Two_Shaft_Reg,
        PreHeat1,
        PreHeat2,
        MainF_Start,
        NotUsed18,
        Keros_FullOn,
        HataliVeri
    }

    enum EnumBaglanti
    {
        BaglantiYok = -1,
        Bagli,
        HataliVeri
    }
}
