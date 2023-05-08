using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableQueueBase), true)]
    public class SerializableQueuePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty Values { get; set; }
        private UnityEditorInternal.ReorderableList Reorderable { get; set; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            if (Reorderable != null)
            {
                totalHeight = Reorderable.GetHeight();
            }
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Values = property.FindPropertyRelative("values");
            if (Reorderable == null)
            {
                Reorderable = new UnityEditorInternal.ReorderableList(property.serializedObject, Values, true, true, true, true);
                Reorderable.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, label);
                Reorderable.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = Reorderable.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("Value");
                    Rect betterRect = new Rect(rect.xMin, rect.yMin + 2.5f, rect.width, rect.height);
                    EditorGUI.PropertyField(betterRect, element, GUIContent.none, true);
                };
                Reorderable.onAddCallback = (UnityEditorInternal.ReorderableList list) => list.serializedProperty.arraySize++;
                Reorderable.elementHeightCallback = (int index) =>
                {
                    SerializedProperty property = Values.GetArrayElementAtIndex(index).FindPropertyRelative("Value");
                    float expandedOffset = property.isExpanded ? 5f : 0f;
                    return EditorGUI.GetPropertyHeight(property, true) + expandedOffset;
                };
            }
            Reorderable.serializedProperty = Values;
            Reorderable.DoList(position);
        }
    }
}
