using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.EditorCoroutines.Editor;

namespace HapticSystem.Editors
{
    [CustomEditor(typeof(HapticClip))]
    public class HapticClipEditor : Editor
    {
        private SerializedProperty amplitude;
        private SerializedProperty useAmplitudeCurve;
        private SerializedProperty amplitudeCurve;
        private SerializedProperty useLowProgressionCurve;
        private SerializedProperty lowProgressionCurve;
        private SerializedProperty useHighProgressionCurve;
        private SerializedProperty highProgressionCurve;
        private SerializedProperty duration;
        private SerializedProperty lowFrequencyMultiplier;
        private SerializedProperty highFrequencyMultiplier;
        private SerializedProperty loop;

        private EditorCoroutine updateCoroutine;
        private EditorCoroutine clipCoroutine;
        private HapticClipInstance hapticClipPlayer;

        public void OnEnable()
        {
            amplitude = serializedObject.FindProperty("strenght");
            useAmplitudeCurve = serializedObject.FindProperty("useGlobalProgressionCurve");
            amplitudeCurve = serializedObject.FindProperty("globalProgressionCurve");
            duration = serializedObject.FindProperty("duration");
            lowFrequencyMultiplier = serializedObject.FindProperty("lowFrequencyMultiplier");
            highFrequencyMultiplier = serializedObject.FindProperty("highFrequencyMultiplier");
            useLowProgressionCurve = serializedObject.FindProperty("useLowProgressionCurve");
            lowProgressionCurve = serializedObject.FindProperty("lowProgressionCurve");
            useHighProgressionCurve = serializedObject.FindProperty("useHighProgressionCurve");
            highProgressionCurve = serializedObject.FindProperty("highProgressionCurve");
            loop = serializedObject.FindProperty("loop");
            updateCoroutine = EditorCoroutineUtility.StartCoroutine(UpdateCoroutine(), this);
        }

        private IEnumerator UpdateCoroutine()
        {
            bool canPlay = true;
            while (true)
            {
                for (int i = 0; i < Gamepad.all.Count; i++)
                {
                    if (Gamepad.all[i].buttonSouth.isPressed && canPlay)
                    {
                        canPlay = false;
                        if (hapticClipPlayer == null || hapticClipPlayer.isPlaying == false)
                        {
                            PlayClip();
                            Repaint();
                        }
                    }
                    else
                    {
                        canPlay = true;
                    }

                    if (hapticClipPlayer != null && hapticClipPlayer.isPlaying)
                    {
                        if (Gamepad.all[i].buttonEast.isPressed)
                        {
                            StopClip();
                            Repaint();
                        }
                    }
                }
                yield return null;
            }
        }

        private void OnDisable()
        {
            EditorCoroutineUtility.StopCoroutine(updateCoroutine);
            StopClip();
        }

        public override void OnInspectorGUI()
        {
            UpdateCoroutine();
            EditorGUILayout.PropertyField(amplitude);
            EditorGUILayout.PropertyField(lowFrequencyMultiplier);
            EditorGUILayout.PropertyField(highFrequencyMultiplier);
            EditorGUILayout.Space();

            DrawProgressionCurve("Global Progression Curve", amplitudeCurve, useAmplitudeCurve);
            DrawProgressionCurve("Low Progression Curve", lowProgressionCurve, useLowProgressionCurve);
            DrawProgressionCurve("High Progression Curve", highProgressionCurve, useHighProgressionCurve);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(loop);
            GUI.enabled = (!loop.boolValue || (loop.boolValue && useAmplitudeCurve.boolValue));
            EditorGUILayout.PropertyField(duration);
            GUI.enabled = true;

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            GUI.enabled = IsAnyGamepadConnected();
            string buttonTooltip = IsAnyGamepadConnected() ? "" : "No controller connected.";

            if (hapticClipPlayer != null && hapticClipPlayer.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("Stop", EditorGUIUtility.IconContent("d_PauseButton On").image, buttonTooltip)))
                    StopClip();
                GUILayout.Label("Or press B on your gamepad");
            }
            else if (hapticClipPlayer == null || hapticClipPlayer.isPlaying == false)
            {
                if (GUILayout.Button(new GUIContent("Play", EditorGUIUtility.IconContent("d_PlayButton On").image, buttonTooltip)))
                    PlayClip();
                GUILayout.Label("Or press A on your gamepad");
            }
            GUI.enabled = true;
        }

        private void DrawProgressionCurve(string label, SerializedProperty curve, SerializedProperty useCurve)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useCurve, new GUIContent(label));
            GUI.enabled = useCurve.boolValue;
            EditorGUILayout.PropertyField(curve, GUIContent.none);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void StopClip()
        {
            if (hapticClipPlayer != null)
                HapticManager.StopClipInstance(hapticClipPlayer);
        }

        public void PlayClip(int gamepadIndex = -1)
        {
            hapticClipPlayer = HapticManager.PlayClipOnGamepadIndex(target as HapticClip, gamepadIndex);
        }

        public bool IsAnyGamepadConnected()
        {
            return (Gamepad.all.Count > 0);
        }
    }
}