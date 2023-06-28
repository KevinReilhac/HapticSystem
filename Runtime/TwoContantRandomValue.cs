using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HapticSystem
{
    [System.Serializable]
    public abstract class TwoConstantRandomValue<T>
    {
        protected enum ValueType
        {
            Constant,
            Random
        }

        [SerializeField] protected ValueType random = ValueType.Constant;
        [SerializeField] protected T minValue = default(T);
        [SerializeField] protected T maxValue = default(T);
        [SerializeField] protected T value = default(T);

        private bool isValueComputed = false;

        public TwoConstantRandomValue(T value)
        {
            minValue = value;
            maxValue = value;
            random = ValueType.Constant;
        }

        public TwoConstantRandomValue(T minValue, T maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;

            random = ValueType.Random;
        }

        public void RecomputeValue()
        {
            if (random == ValueType.Random)
                value = GetRandomValue();
            isValueComputed = true;
        }

        public T GetValue()
        {
            if (random == ValueType.Random && !isValueComputed)
                RecomputeValue();
            return (value);
        }
        public bool IsRandom => random == ValueType.Random;

        abstract protected T GetRandomValue();
    }

    [System.Serializable]
    public class TwoConstantRandomFloat : TwoConstantRandomValue<float>
    {
        public TwoConstantRandomFloat(float minValue, float maxValue) : base(minValue, maxValue) { }
        public TwoConstantRandomFloat(float value) : base(value) { }

        protected override float GetRandomValue()
        {
            return (Random.Range(minValue, maxValue));
        }
        public static implicit operator TwoConstantRandomFloat(float value) => new TwoConstantRandomFloat(value);
    }

    [System.Serializable]
    public class TwoConstantRandomFloat01 : TwoConstantRandomValue<float>
    {
        public TwoConstantRandomFloat01(float minValue, float maxValue) : base(Mathf.Clamp01(minValue), Mathf.Clamp01(maxValue)) { }
        public TwoConstantRandomFloat01(float value) : base(Mathf.Clamp01(value)) { }

        protected override float GetRandomValue()
        {
            return (Random.Range(minValue, maxValue));
        }

        public static implicit operator TwoConstantRandomFloat01(float value) => new TwoConstantRandomFloat01(value);
    }

    [System.Serializable]
    public class TwoConstantRandomInt : TwoConstantRandomValue<int>
    {
        public TwoConstantRandomInt(int value) : base(value) { }
        public TwoConstantRandomInt(int minValue, int maxValue) : base(minValue, maxValue) { }

        protected override int GetRandomValue()
        {
            return (Random.Range(minValue, maxValue));
        }

        public static implicit operator TwoConstantRandomInt(int value) => new TwoConstantRandomInt(value);
    }
}