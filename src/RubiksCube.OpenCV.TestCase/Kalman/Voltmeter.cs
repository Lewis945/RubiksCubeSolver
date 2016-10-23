using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.Kalman
{
    public class Voltmeter
    {
        private float _trueVoltage;
        private float _noise;
        private Random _random;

        public Voltmeter(float trueVoltage, float noise)
        {
            _random = new Random();

            _trueVoltage = trueVoltage;
            _noise = noise;
        }

        public float GetVoltage()
        {
            return _trueVoltage;
        }

        public float GetVoltageWithNoise()
        {
            int random = _random.Next((int)(_trueVoltage - _noise), (int)(_trueVoltage + _noise));
            float noise = 0.5f;
            float result = random + noise > _trueVoltage + _noise ? random - noise : random + noise;
            return result;
        }
    }
}
