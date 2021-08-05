using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty ticks = property.FindPropertyRelative("ticks");
            SerializedProperty kind = property.FindPropertyRelative("kind");

            DateTime datetime = new DateTime(ticks.longValue, (DateTimeKind)kind.intValue);

            Rect labelRect = new Rect(position.xMin, position.yMin, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect dateRect = new Rect(labelRect.xMax, labelRect.yMin, (position.width - EditorGUIUtility.labelWidth) - 50f, EditorGUIUtility.singleLineHeight);
            Rect kindRect = new Rect(dateRect.xMax, dateRect.yMin, 50f, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);
            GUI.enabled = false;
            EditorGUI.TextField(dateRect, datetime.ToString());
            EditorGUI.TextField(kindRect, datetime.Kind.ToString());
            GUI.enabled = true;
        }
    }
}
