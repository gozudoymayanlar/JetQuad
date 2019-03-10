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

namespace JetMotorTestUI.Models
{
    class KontrolPaneliModel : INotifyPropertyChanged
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
        ObservableCollection<string> _comPortCollection = new ObservableCollection<string> { };
        SerialPort _mySerialPort;
        System.Timers.Timer _timer;
        string _seciliComPort;
        string _baudRate = "38400";
        bool _kayitYap = false;
        EnumBaglanti _baglanti = EnumBaglanti.BaglantiYok;
        EnumMotorStatus _motorStatus = EnumMotorStatus.BaglantiYok;

        int _kumandaThrottle = 50;
        int _kumandaTrim = 20;
        int _arduThrottle = 51;
        int _arduTrim = 21;
        int _motorThrottle = 52;

        int _egt = 87;

        float _thrust = 30.0f;

        long _referenceRPM = 6000;
        long _motorRPM = 6200;
        float _motorRPM_gauge = 6.2f;

        float _fuel = 30f;
        float _batteryVoltage = 11.2f;
        float _pumpVoltage = 15.4f;

        long _time = 0;
        #endregion

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
                    MotorStatus = EnumMotorStatus.BaglantiYok;
                }
            }
        }
        public EnumMotorStatus MotorStatus
        {
            get { return _motorStatus; }
            set { _motorStatus = value; OnPropertyChanged(nameof(MotorStatus)); }
        }

        /// <summary>
        /// Arayüzde sliderla belirlenen throttle değeri. Bu değer arduya gönderiliyor.
        /// </summary>
        public int KumandaThrottle
        {
            get { return _kumandaThrottle; }
            set { _kumandaThrottle = value; OnPropertyChanged(nameof(KumandaThrottle)); }
        }

        /// <summary>
        /// Arayüzde sliderla belirlenen trim değeri. Bu değer arduya gönderiliyor.
        /// </summary>
        public int KumandaTrim
        {
            get { return _kumandaTrim; }
            set { _kumandaTrim = value; OnPropertyChanged(nameof(KumandaTrim)); }
        }

        /// <summary>
        /// Arduya gönderilen KumandaThrottle değerinin echo olarak ardudan alınan throttle değeri. Bu değer ardudan alınıyor.
        /// </summary>
        public int ArduThrottle
        {
            get { return _arduThrottle; }
            set { _arduThrottle = value; OnPropertyChanged(nameof(ArduThrottle)); }
        }

        /// <summary>
        /// Arduya gönderilen KumandaTrim değerinin echo olarak ardudan alınan trim değeri. Bu değer ardudan alınıyor.
        /// </summary>
        public int ArduTrim
        {
            get { return _arduTrim; }
            set { _arduTrim = value; OnPropertyChanged(nameof(ArduTrim)); }
        }

        /// <summary>
        /// Ardunun Jet motor ECU'sundan okuduğu throttle değeri. Bu değer ardudan okunuyor.
        /// </summary>
        public int MotorThrottle
        {
            get { return _motorThrottle; }
            set { _motorThrottle = value; OnPropertyChanged(nameof(MotorThrottle)); }
        }

        public int EGT
        {
            get { return _egt; }
            set { _egt = value; OnPropertyChanged(nameof(EGT)); }
        }
        public float Thrust
        {
            get { return _thrust; }
            set { _thrust = value; OnPropertyChanged(nameof(Thrust)); }
        }
        public long ReferenceRPM
        {
            get { return _referenceRPM; }
            set { _referenceRPM = value; OnPropertyChanged(nameof(ReferenceRPM)); }
        }
        public long MotorRPM
        {
            get { return _motorRPM; }
            set { _motorRPM = value; OnPropertyChanged(nameof(MotorRPM)); }
        }
        public float MotorRPM_gauge
        {
            get { return _motorRPM_gauge; }
            set { _motorRPM_gauge = value; OnPropertyChanged(nameof(MotorRPM_gauge)); }
        }
        public float Fuel
        {
            get { return _fuel; }
            set { _fuel = value; OnPropertyChanged(nameof(Fuel)); }
        }
        public float BatteryVoltage
        {
            get { return _batteryVoltage; }
            set { _batteryVoltage = value; OnPropertyChanged(nameof(BatteryVoltage)); }
        }
        public float PumpVoltage
        {
            get { return _pumpVoltage; }
            set { _pumpVoltage = value; OnPropertyChanged(nameof(PumpVoltage)); }
        }
        public long Time
        {
            get { return _time; }
            set { _time = value; OnPropertyChanged(nameof(Time)); }
        }
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
