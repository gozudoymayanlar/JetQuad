using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AttitudeControlTestUI.Models
{
    class MyVeri : BaseViewModel
    {
        private float _degerFloat;
        private byte _degerByte;
        private float _max;
        private float _min;
        private bool _byteDegisti = false;
        private int _idx;


        public MyVeri(float deger, float min, float max, int idx = 0)
        {
            _min = min;
            _max = max;
            _idx = idx;
            DegerFloat = deger;
        }

        private byte map2byte(float toMin = 0, float toMax = 254)
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

            int result = Convert.ToInt16(toAbs + toMin);
            return Convert.ToByte(result);
        }

        private float map2float(float fromMin = 0, float fromMax = 254)
        {
            float fromAbs = Convert.ToSingle(_degerByte) - fromMin;
            float fromMaxAbs = fromMax - fromMin;

            float normal = fromAbs / fromMaxAbs;

            float toMaxAbs = _max - _min;
            float toAbs = toMaxAbs * normal;

            float to = toAbs + _min;
            return to;
        }

        public float DegerFloat
        {
            get
            {
                return _degerFloat;
            }

            set
            {
                _degerFloat = value;
                if (!_byteDegisti)
                {
                    _degerByte = map2byte();
                }
                OnPropertyChanged(nameof(DegerFloat));
                
            }
        }

        public byte DegerByte
        {
            get
            {
                return _degerByte;
            }

            set
            {
                _degerByte = value;
                _byteDegisti = true;
                DegerFloat = map2float();
                _byteDegisti = false;
            }
        }

        public float Max
        {
            get
            {
                return _max;
            }

            set
            {
                _max = value;
            }
        }

        public float Min
        {
            get
            {
                return _min;
            }

            set
            {
                _min = value;
            }
        }

        public int Idx
        {
            get { return _idx; }
            set { _idx = value; }
        }
    }
}
