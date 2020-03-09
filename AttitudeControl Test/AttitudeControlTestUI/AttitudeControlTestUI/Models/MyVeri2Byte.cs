using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AttitudeControlTestUI.Models
{
    class MyVeri2Byte : BaseViewModel
    {
        private float _degerFloat;
        private ushort _degerUshort;
        private float _max;
        private float _min;
        private bool _ushortDegisti = false;
        private int _idx;


        public MyVeri2Byte(float deger, float min, float max, int idx = 0)
        {
            _min = min;
            _max = max;
            _idx = idx;
            DegerFloat = deger;
        }

        private ushort map2ushort(float toMin = 0, float toMax = 254)
        {
            if (!(_degerFloat >= _min && _degerFloat <= _max))
            {
                return 0;
            }

            float fromAbs = _degerFloat - _min;
            float fromMaxAbs = _max - _min;

            float normal = fromAbs / fromMaxAbs;

            float toMaxAbs = toMax - toMin;
            float toAbs = toMaxAbs * normal;

            return Convert.ToUInt16(toAbs + toMin);
        }

        private float map2float(float fromMin = 0, float fromMax = 65535)
        {
            float fromAbs = Convert.ToSingle(_degerUshort) - fromMin;
            float fromMaxAbs = fromMax - fromMin;

            float normal = fromAbs / fromMaxAbs;

            float toMaxAbs = _max - _min;
            float toAbs = toMaxAbs * normal;

            float to = toAbs + _min;
            return to;
        }

        public float DegerFloat
        {
            get {  return _degerFloat;}
            set
            {
                _degerFloat = value;
                if (!_ushortDegisti)
                {
                    _degerUshort = map2ushort();
                }
                OnPropertyChanged(nameof(DegerFloat));
                
            }
        }

        public ushort DegerUshort
        {
            get
            {
                return _degerUshort;
            }

            set
            {
                _degerUshort = value;
                _ushortDegisti = true;
                DegerFloat = map2float();
                _ushortDegisti = false;
            }
        }

        public float Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public float Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int Idx
        {
            get { return _idx; }
            set { _idx = value; }
        }
    }
}
