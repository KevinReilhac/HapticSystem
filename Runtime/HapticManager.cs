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
        private const float UPDATE_MOTORS_SPEEDS_INTERVAL = 0.01f;
        internal static Dictionary<int, MotorsSpeed> currentSpeeds = new Dictionary<int, MotorsSpeed>();
        internal static Dictionary<int, MotorsSpeed> lastPlayedSpeeds = new Dictionary<int, MotorsSpeed>();
        internal static Dictionary<int, List<HapticClipInstance>> playingClips = new Dictionary<int, List<HapticClipInstance>>();

        private static float _strenghtMultiplier = 1f;
        private static List<int> askedTargetsUpdateMotors = new List<int>();

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
        public static HapticClipInstance PlayClipOnGamepadIndex(HapticClip clip, int targetGamepadIndex = -1, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            HapticClipInstance clipPlayer = new HapticClipInstance(clip, targetGamepadIndex, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier);

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

        public static HapticClipInstance PlayClipOnAllGamepads(HapticClip clip, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            return (PlayClipOnGamepadIndex(clip, -1, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier));
        }

        /// <summary>
        /// Play a clip and return an Haptic Clip instance
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="gamepad">Target gamepad</param>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(HapticClip clip, Gamepad gamepad, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            if (gamepad == null)
                return (null);
            return PlayClipOnGamepadIndex(clip, Gamepad.all.IndexOf(g => g == gamepad));
        }

        /// <summary>
        /// Play a clip and return an Haptic Clip instance
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="gamepad">Target gamepad</param>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(HapticClip clip, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            return (PlayClip(clip, Gamepad.current, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier));
        }

        /// <summary>
        /// Play a clip and return an Haptic Clip instance
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="deviceID">Target device </param>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(HapticClip clip, int deviceID, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            InputDevice device = InputSystem.GetDeviceById(deviceID);
            return PlayClip(clip, device, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier);
        }

        /// <summary>
        /// Play a clip and return an Haptic Clip instance
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="playerIndex">Target playerInput</param>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(HapticClip clip, PlayerInput playerInput, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            InputDevice device = null;

            if (playerInput.devices.Count == 0)
                return null;
            device = playerInput.devices[0].device;
            return (PlayClip(clip, device, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier));
        }

        /// <summary>
        /// Play a clip and return an Haptic Clip instance
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="device">Target device </param>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClipInstance PlayClip(HapticClip clip, InputDevice device, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            if (device is Gamepad gamepad)
                return (PlayClip(clip, gamepad, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier));
            return (null);
        }

        /// <summary>
        /// Create and play haptic clip
        /// </summary>
        /// <returns> Use HapticClipInstance to stop clip by using HapticManager.Stop method </returns>
        public static HapticClip CreateCustomClip(TwoConstantRandomFloat01 strenght,
            float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f,
            bool useProgressionCurve = false, AnimationCurve progressionCurve = null,
            bool loop = false, float duration = 0.3f)
        {
            return ScriptableObject.CreateInstance<HapticClip>().Setup(
                    strenght,
                    lowFrequencyMultiplier,
                    highFrequencyMultiplier,
                    useProgressionCurve,
                    progressionCurve,
                    loop,
                    duration
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
                    AskUpdateMotorsSpeeds(targetGamepad);
                }
            }
            else
            {
                currentSpeeds.Add(targetGamepad, new MotorsSpeed(newLowFrequency, newHighFrequency));
                AskUpdateMotorsSpeeds(targetGamepad);
            }
        }

        internal static void AskUpdateMotorsSpeeds(int targetGamepad)
        {
            if (!askedTargetsUpdateMotors.Contains(targetGamepad))
                askedTargetsUpdateMotors.Add(targetGamepad);
        }

        internal static IEnumerator UpdateMotorsSpeedsCoroutine()
        {
            List<int> targetsToUpdate = null;
            while (true)
            {
                targetsToUpdate = new List<int>(askedTargetsUpdateMotors);
                foreach (int targetGamepad in targetsToUpdate)
                {
                    UpdateMotorsSpeedsForTarget(targetGamepad);
                    yield return null;
                }
                askedTargetsUpdateMotors.Clear();
                yield return new WaitForSeconds(UPDATE_MOTORS_SPEEDS_INTERVAL);
            }
        }

        internal static void UpdateMotorsSpeedsForTarget(int targetGamepad)
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
            if (lastPlayedSpeeds.TryGetValue(targetGamepad, out MotorsSpeed lastPlayed))
            {
                if (lastPlayed.LowFrequency == lowFrequency && lastPlayed.HighFrequency == highFrequency)
                    return;
            }
            Debug.LogFormat("UpdateMotorsSpeedsForTarget {0} {1} {2}", targetGamepad, lowFrequency, highFrequency);
            Gamepad.all[targetGamepad].SetMotorSpeeds(lowFrequency, highFrequency);

            if (!lastPlayedSpeeds.ContainsKey(targetGamepad))
                lastPlayedSpeeds.Add(targetGamepad, new MotorsSpeed(lowFrequency, highFrequency));
            else
                lastPlayedSpeeds[targetGamepad] = new MotorsSpeed(lowFrequency, highFrequency);
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
                    _coroutinePlayer.StartCoroutine(UpdateMotorsSpeedsCoroutine());
                }

                return (_coroutinePlayer);
            }
        }

        internal static Coroutine StartCoroutine(IEnumerator routine)
        {
            Debug.Log("StartCoroutine " + routine.ToString());
            return CoroutinePlayer.StartCoroutine(routine);
        }

        internal static void StopCoroutine(Coroutine routine)
        {
            CoroutinePlayer.StopCoroutine(routine);
        }
        #endregion
    }
}