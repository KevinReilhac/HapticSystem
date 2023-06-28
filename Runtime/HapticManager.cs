using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HapticSystem
{
    /// <summary>
    /// Main class that play and stop HapticClips
    /// </summary>
    public static class HapticManager
    {
        private static Dictionary<int, MotorsSpeed> currentSpeeds = new Dictionary<int, MotorsSpeed>();
        private static Dictionary<int, List<HapticClipInstance>> playingClips = new Dictionary<int, List<HapticClipInstance>>();

        private static float _strenghtMultiplier = 1f;

        #region PUBLICS
        /// <summary>
        /// Global strenght multiplier
        /// Use it for a vibration settings slider for exemple
        /// (Value between 0 and 1)
        /// </summary>
        /// <value></value>
        public static float StrenghtMultiplier
        {
            get => _strenghtMultiplier;
            set => _strenghtMultiplier = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Play a clip and return an Haptic Clip instance
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="targetGamepadIndex">Target gamepad (-1 for all gamepads) </param>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(HapticClip clip, int targetGamepadIndex = -1)
        {
            HapticClipInstance clipPlayer = new HapticClipInstance(clip, targetGamepadIndex);

            if (targetGamepadIndex == -1)
            {
                for (int i = 0; i < Gamepad.all.Count; i++)
                    AddPlayingClip(i, clipPlayer);
                return (clipPlayer);
            }

            if (targetGamepadIndex < 0 || targetGamepadIndex >= Gamepad.all.Count)
            {
                Debug.LogErrorFormat("{0} is invalid gamepad index", targetGamepadIndex);
                return (null);
            }

            AddPlayingClip(targetGamepadIndex, clipPlayer);
            return (clipPlayer);
        }

        /// <summary>
        /// Create and play haptic clip
        /// </summary>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(TwoConstantRandomFloat01 strenght,
            float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f,
            bool useProgressionCurve = false, AnimationCurve progressionCurve = null,
            bool loop = false, float duration = 0.3f,
            int targetGamepadIndex = -1)
        {
            return PlayClip
            (
                ScriptableObject.CreateInstance<HapticClip>().Setup
                (
                    strenght,
                    lowFrequencyMultiplier,
                    highFrequencyMultiplier,
                    useProgressionCurve,
                    progressionCurve,
                    loop,
                    duration
                ),
                targetGamepadIndex
            );
        }

        /// <summary>
        /// Stop a clip instance
        /// </summary>
        /// <param name="clipInstance">Clip instance to stop</param>
        public static void StopClipInstance(HapticClipInstance clipInstance)
        {
            if (clipInstance == null)
                return;
            clipInstance.Stop();
            if (clipInstance.targetGamepadIndex == -1)
            {
                foreach (var item in playingClips)
                    item.Value.Remove(clipInstance);
            }
            else
            {
                RemovePlayingClip(clipInstance.targetGamepadIndex, clipInstance);
            }

            RecomputeSpeeds();
        }

        /// <summary>
        /// Stop every clip instances
        /// </summary>
        public static void StopAllClipInstances()
        {
            playingClips.Clear();
            RecomputeSpeeds();
        }

        #endregion

        #region INTERNALS
        internal static void RecomputeSpeeds(int targetGamepad = -1)
        {
            if (targetGamepad == -1)
            {
                for (int i = 0; i < Gamepad.all.Count; i++)
                    RecomputeSpeeds(i);
                return;
            }

            float newLowFrequency = 0;
            float newHighFrequency = 0;

            if (playingClips.TryGetValue(targetGamepad, out List<HapticClipInstance> targetPlayingClips))
            {
                foreach (HapticClipInstance clipPlayer in targetPlayingClips)
                {
                    clipPlayer.EvaluateStrenghts(out float lowFrequency, out float highFrequency);

                    newLowFrequency = Mathf.Max(lowFrequency, newLowFrequency);
                    newHighFrequency = Mathf.Max(highFrequency, newHighFrequency);
                }
            }

            if (currentSpeeds.TryGetValue(targetGamepad, out MotorsSpeed currentMotorSpeed))
            {
                if (newLowFrequency != currentMotorSpeed.LowFrequency || newHighFrequency != currentMotorSpeed.HighFrequency)
                {
                    currentMotorSpeed.LowFrequency = newLowFrequency;
                    currentMotorSpeed.HighFrequency = newHighFrequency;
                    UpdateMotorsSpeeds(targetGamepad);
                }
            }
            else
            {
                currentSpeeds.Add(targetGamepad, new MotorsSpeed(newLowFrequency, newHighFrequency));
                UpdateMotorsSpeeds(targetGamepad);
            }
        }

        internal static void UpdateMotorsSpeeds(int targetGamepad)
        {
            float lowFrequency = 0;
            float highFrequency = 0;
            if (currentSpeeds.TryGetValue(targetGamepad, out MotorsSpeed motorsSpeed))
            {
                lowFrequency = motorsSpeed.LowFrequency;
                highFrequency = motorsSpeed.HighFrequency;
            }

            if (targetGamepad < 0 || targetGamepad >= Gamepad.all.Count)
            {
                Debug.LogErrorFormat("{0} gamepad index is invalid.", targetGamepad);
                return;
            }
            Gamepad.all[targetGamepad].SetMotorSpeeds(lowFrequency, highFrequency);
        }


        #endregion

        #region PRIVATES
        private static void AddPlayingClip(int targetGamepadIndex, HapticClipInstance clipPlayer)
        {
            if (playingClips.TryGetValue(targetGamepadIndex, out List<HapticClipInstance> targetPlayingClips))
            {
                targetPlayingClips.Add(clipPlayer);
            }
            else
            {
                playingClips.Add(targetGamepadIndex, new List<HapticClipInstance>() { clipPlayer });
            }

            RecomputeSpeeds();
        }

        private static void RemovePlayingClip(int targetGamepadIndex, HapticClipInstance clipPlayer)
        {
            if (playingClips.TryGetValue(targetGamepadIndex, out List<HapticClipInstance> targetPlayingClips))
            {
                targetPlayingClips.Remove(clipPlayer);
                RecomputeSpeeds();
            }
        }
        #endregion

        #region Coroutines
        private static HapticManagerCoroutinePlayer _coroutinePlayer = null;
        private static HapticManagerCoroutinePlayer CoroutinePlayer
        {
            get
            {
                if (_coroutinePlayer == null)
                {
                    GameObject go = new GameObject("CoroutinePlayer");
                    _coroutinePlayer = go.AddComponent<HapticManagerCoroutinePlayer>();
                }

                return (_coroutinePlayer);
            }
        }

        internal static Coroutine StartCoroutine(IEnumerator routine)
        {
            return CoroutinePlayer.StartCoroutine(routine);
        }

        internal static void StopCoroutine(Coroutine routine)
        {
            CoroutinePlayer.StopCoroutine(routine);
        }
        #endregion

    }

    public class HapticManagerCoroutinePlayer : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}