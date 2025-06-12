using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HapticSystem
{
    [CreateAssetMenu(menuName = "Haptic/HapticClip", fileName = "NewHapticClip")]
    public class HapticClip : ScriptableObject
    {
        [Tooltip("Vibration strenght")]
        [SerializeField] private TwoConstantRandomFloat01 strenght = new TwoConstantRandomFloat01(0.2f);
        [Tooltip("Small vibrator strenght multiplier")]
        [SerializeField] [Range(0f, 1f)] public float lowFrequencyMultiplier = 1f;
        [Tooltip("Big vibrator strenght multiplier")]
        [SerializeField] [Range(0f, 1f)] public float highFrequencyMultiplier = 1f;
        [SerializeField] private bool useProgressionCurve = false;
        [SerializeField] [BoundedCurve] private AnimationCurve progressionCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
        [SerializeField] private bool loop = false;
        [SerializeField] [Min(0.01f)] private float duration = 0.3f;

        internal HapticClip Setup(TwoConstantRandomFloat01 strenght,
            float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f,
            bool useProgressionCurve = false, AnimationCurve progressionCurve = null,
            bool loop = false, float duration = 0.3f)
        {
            this.strenght = strenght;
            this.lowFrequencyMultiplier = Mathf.Clamp01(lowFrequencyMultiplier);
            this.highFrequencyMultiplier = Mathf.Clamp01(highFrequencyMultiplier);
            this.useProgressionCurve = useProgressionCurve;
            this.progressionCurve = progressionCurve;
            this.loop = loop;
            this.duration = duration;

            return (this);
        }

        internal void RecomputeRandom()
        {
            strenght.RecomputeValue();
        }

        internal void EvaluateStrenghts(float progress, out float lowFrequency, out float highFrequency)
        {
            if (!useProgressionCurve || progressionCurve == null)
            {
                lowFrequency = strenght.GetValue() * HapticManager.StrenghtMultiplier * lowFrequencyMultiplier;
                highFrequency = strenght.GetValue() * HapticManager.StrenghtMultiplier * highFrequencyMultiplier;
                return;
            }

            progress = Mathf.Clamp01(progress);
            float progressionCurveEvaluation = progressionCurve.Evaluate(progress);

            lowFrequency = progressionCurveEvaluation * strenght.GetValue() * HapticManager.StrenghtMultiplier * lowFrequencyMultiplier;
            highFrequency = progressionCurveEvaluation * strenght.GetValue() * HapticManager.StrenghtMultiplier * highFrequencyMultiplier;
        }

        public float Duration => Mathf.Max(duration, 0.01f); // Prevent division by 0
        public bool UseProgressionCurve => useProgressionCurve;
        public bool Loop => loop;
    }

}