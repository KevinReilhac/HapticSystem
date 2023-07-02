using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

namespace HapticSystem.Editors
{
    [CustomPropertyDrawer(typeof(HapticClip))]
    public class HapticClipPropertyDrawer : PropertyDrawer
    {
        private const float BUTTON_WIDTH = 20f;
        private const float SPACE = 3f;
        private HapticClipInstance clipInstance = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float propertyWidth = position.width;

            if (property.objectReferenceValue != null)
                propertyWidth -= BUTTON_WIDTH;
            EditorGUI.PropertyField(new Rect(position.x, position.y, propertyWidth, EditorGUIUtility.singleLineHeight), property, new GUIContent(property.displayName));
            position.x += (position.width - BUTTON_WIDTH) + SPACE;

            GUI.enabled = Gamepad.all.Count > 0;
            if (property.objectReferenceValue != null)
                DrawPlayButton(position, property.objectReferenceValue as HapticClip);
            GUI.enabled = false;
        }

        private void DrawPlayButton(Rect position, HapticClip clip)
        {
            bool isPlaying = clipInstance != null && clipInstance.isPlaying;

            string iconString = isPlaying ? "d_PauseButton On" : "d_PlayButton On";
            if (GUI.Button(new Rect(position.x, position.y, BUTTON_WIDTH, EditorGUIUtility.singleLineHeight), EditorGUIUtility.IconContent(iconString)))
            {
                if (isPlaying)
                {
                    HapticManager.StopClipInstance(clipInstance);
                }
                else
                {
                    clipInstance = HapticManager.PlayClipOnAllGamepads(clip);
                }
            }
        }

        private void OnDisable()
        {
            if (clipInstance != null)
                HapticManager.StopClipInstance(clipInstance);
        }
    }
}