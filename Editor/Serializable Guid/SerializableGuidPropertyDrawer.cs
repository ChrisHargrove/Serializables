using UnityEngine;
using UnityEditor;
using System;

using BatteryAcid.Serializables;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        private class Styles
        {
            public static GUIStyle PadLock = "IN LockButton";
        }

        private bool IsLocked { get; set; } = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsLocked ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLine = EditorGUIUtility.singleLineHeight;

            Rect labelRect;
            Rect padLockRect;
            Rect fieldRect;

            if (!string.IsNullOrEmpty(label.text))
            {
                float w0 = EditorGUIUtility.labelWidth;
                float w1 = position.width - w0;
                labelRect = new Rect(position.xMin, position.yMin, w0, singleLine);
                padLockRect = new Rect(labelRect.xMax - singleLine, labelRect.yMin + 1.5f, singleLine, singleLine);
                fieldRect = new Rect(labelRect.xMax, position.yMin, w1, singleLine);
                EditorGUI.LabelField(labelRect, label);
            }
            else
            {
                padLockRect = new Rect(position.xMin, position.yMin + 1.5f, singleLine, singleLine);
                fieldRect = new Rect(position.xMin + 5f, position.yMin, position.width - 5f, singleLine);

                Debug.Log($"Padlock X Start {padLockRect.xMin}");
                Debug.Log($"Field X Start {fieldRect.xMin}");
            }

            IsLocked = GUI.Toggle(padLockRect, IsLocked, GUIContent.none, Styles.PadLock);

            using (EditorGUI.DisabledGroupScope disabled = new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("guidString"), GUIContent.none);
            }

            if (!IsLocked)
            {
                Rect generateRect = new Rect(fieldRect.xMin, fieldRect.yMax, fieldRect.width, singleLine);
                if (GUI.Button(generateRect, "Generate Guid"))
                {
                    property.FindPropertyRelative("guidString").stringValue = Guid.NewGuid().ToString();
                }
            }
        }
    }
}
