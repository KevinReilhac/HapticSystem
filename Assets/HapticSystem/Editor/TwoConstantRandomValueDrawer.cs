using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HapticSystem.Editors
{
    [CustomPropertyDrawer(typeof(TwoConstantRandomValue<>), true)]
    class TwoConstantRandomValueDrawer : PropertyDrawer
    {

        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty random = property.FindPropertyRelative("random");
            SerializedProperty minValue = property.FindPropertyRelative("minValue");
            SerializedProperty maxValue = property.FindPropertyRelative("maxValue");
            SerializedProperty value = property.FindPropertyRelative("value");

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float valueDrawWidth = (position.width - 20f) / 2f - 5f;
            Rect randomRect = new Rect(position.x + position.width - 20f, position.y, 20f, position.height);
            Rect minValueRect = new Rect(position.x, position.y, valueDrawWidth, position.height);
            Rect minValueRectExtended = new Rect(position.x, position.y, position.width - 25f, position.height);
            Rect maxValueRect = new Rect(position.x + valueDrawWidth + 5f, position.y, valueDrawWidth, position.height);


            if (random.boolValue)
            {
                EditorGUI.PropertyField(minValueRect, minValue, GUIContent.none);
                EditorGUI.PropertyField(maxValueRect, maxValue, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(minValueRectExtended, value, GUIContent.none);
            }

            EditorGUI.PropertyField(randomRect, random, GUIContent.none);
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(TwoConstantRandomFloat01), true)]
    class TwoConstantRandomFloat01Drawer : PropertyDrawer
    {

        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty random = property.FindPropertyRelative("random");
            SerializedProperty minValue = property.FindPropertyRelative("minValue");
            SerializedProperty maxValue = property.FindPropertyRelative("maxValue");
            SerializedProperty value = property.FindPropertyRelative("value");

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float valueDrawWidth = (position.width - 20f) / 2f - 5f;
            Rect randomRect = new Rect(position.x + position.width - 20f, position.y, 20f, position.height);
            Rect minValueRect = new Rect(position.x, position.y, valueDrawWidth, position.height);
            Rect minValueRectExtended = new Rect(position.x, position.y, position.width - 25f, position.height);
            Rect maxValueRect = new Rect(position.x + valueDrawWidth + 5f, position.y, valueDrawWidth, position.height);


            if (random.boolValue)
            {
                EditorGUI.Slider(minValueRect, minValue, 0f, 1f, GUIContent.none);
                EditorGUI.Slider(maxValueRect, maxValue, 0f, 1f, GUIContent.none);
            }
            else
            {
                EditorGUI.Slider(minValueRectExtended, value, 0f, 1f, GUIContent.none);
            }

            EditorGUI.PropertyField(randomRect, random, GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}