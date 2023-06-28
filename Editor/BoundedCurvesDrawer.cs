using UnityEngine;
using UnityEditor;

namespace HapticSystem.Editors
{
    [CustomPropertyDrawer(typeof(BoundedCurveAttribute))]
    public class BoundedCurveDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            BoundedCurveAttribute boundedCurve = (BoundedCurveAttribute)attribute;
            return EditorGUIUtility.singleLineHeight * boundedCurve.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BoundedCurveAttribute boundedCurve = (BoundedCurveAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);
            property.animationCurveValue = EditorGUI.CurveField(
              position,
              label,
              property.animationCurveValue,
              Color.green,
              boundedCurve.bounds
             );
            EditorGUI.EndProperty();
        }
    }
}