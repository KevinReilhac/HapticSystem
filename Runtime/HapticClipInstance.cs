using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif


namespace HapticSystem
{
    public class HapticClipInstance
    {
        public HapticClip clip {get; private set;} = null;
        public int targetGamepadIndex {get; private set;} = -1;
        public bool isPlaying {get; private set;} = false;
        public float progress {get; private set;} = 0;
        public float speedMultiplier {get; set;} = 1f;
        public float strenghtMultiplier {get; set;} = 1f;
        public float lowFrequencyMultiplier {get; set;} = 1f;
        public float highFrequencyMultiplier {get; set;} = 1f;

        private Coroutine coroutine;
        #if UNITY_EDITOR
        private EditorCoroutine editorCoroutine;
        #endif


        internal void EvaluateStrenghts(out float lowFrequency, out float highFrequency)
        {
            float progress = this.progress * speedMultiplier;
            if (progress > 1f)
                progress = progress % 1f;
            clip.EvaluateStrenghts(progress, out float evalLowFrequency, out float evalHighFrequency);
            lowFrequency = evalLowFrequency * strenghtMultiplier * lowFrequencyMultiplier;
            highFrequency = evalHighFrequency * strenghtMultiplier * highFrequencyMultiplier;
        }

        /// <summary>
        /// Create a new HapticClip instance
        /// /!\ Use HapticManager.Playinstead /!\
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="targetGamepadIndex"></param>
        public HapticClipInstance(HapticClip clip, int targetGamepadIndex = -1, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            this.targetGamepadIndex = targetGamepadIndex;
            this.clip = clip;
            this.strenghtMultiplier = strenghtMultiplier;
            this.lowFrequencyMultiplier = lowFrequencyMultiplier;
            this.highFrequencyMultiplier = highFrequencyMultiplier;
            clip.RecomputeRandom();
            StartHapticClipUpdateCoroutine();
        }

        private void StartHapticClipUpdateCoroutine()
        {
            isPlaying = true;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                editorCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(HapticClipUpdate());
            else
                coroutine = HapticManager.StartCoroutine(HapticClipUpdate());
            #else
            coroutine = HapticManager.StartCoroutine(HapticClipUpdate());
            #endif
        }

        internal IEnumerator HapticClipUpdate()
        {
            float endTime = 0f;
            float startTime = 0f;

            do
            {
                endTime = Time.realtimeSinceStartup + clip.Duration;
                startTime = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup < endTime)
                {
                    if (clip.UseProgressionCurve)
                    {
                        progress = (Time.realtimeSinceStartup - startTime) / clip.Duration;
                        HapticManager.RecomputeSpeeds();
                    }
                    yield return null;
                }
            } while(clip.Loop);


            HapticManager.StopClipInstance(this);
        }

        internal void Stop()
        {
            isPlaying = false;

            #if UNITY_EDITOR
            if (!Application.isPlaying && editorCoroutine != null)
                EditorCoroutineUtility.StopCoroutine(editorCoroutine);
            else if (coroutine != null)
                HapticManager.StopCoroutine(coroutine);
            #else
            if (coroutine != null)
                HapticManager.StopCoroutine(coroutine);
            #endif

        }
    }
}