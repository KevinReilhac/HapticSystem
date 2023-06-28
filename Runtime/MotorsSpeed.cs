using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HapticSystem
{
    internal class MotorsSpeed
    {
        private float _lowFrequency = 0f;
        private float _highFrequency = 0f;

        public float LowFrequency
        {
            get => _lowFrequency;
            set => _lowFrequency = Mathf.Clamp01(value);
        }

        public float HighFrequency
        {
            get => _highFrequency;
            set => _highFrequency = Mathf.Clamp01(value);
        }

        public MotorsSpeed(float lowFrequency, float highFrequency)
        {
            Set(lowFrequency, highFrequency);
        }

        public void Set(float lowFrequency, float highFrequency)
        {
            this._lowFrequency = lowFrequency;
            this._highFrequency = highFrequency;
        }
    }
}