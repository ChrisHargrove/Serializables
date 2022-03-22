using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableHashSetBase), true)]
    public class SerializableHashSetPropertyDrawer : SerializableHashSetConflictPropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                SerializedProperty values = property.FindPropertyRelative("values");

                for (int i = 0; i < values.arraySize; i++)
                {
                    SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");
                    totalHeight += EditorGUI.GetPropertyHeight(value, value.isArray);
                }
                totalHeight += 5;

                SerializableConflict conflict = GetConflict(property);
                if (conflict.Index != -1)
                {
                    totalHeight += conflict.LineHeight;
                }
            }
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty values = property.FindPropertyRelative("values");

            SerializableConflict conflict = GetConflict(property);
            InsertConflict(values, conflict);

            label = EditorGUI.BeginProperty(position, label, property);
            Rect foldout = Utilities.GetNextPropertyRect(ref position);
            if (property.isExpanded = EditorGUI.Foldout(foldout, property.isExpanded, label))
            {
                Rect plusRect = new Rect(foldout.x + (foldout.width - EditorGUIUtility.singleLineHeight), foldout.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                if (!conflict.IsConflicting && GUI.Button(plusRect, Styles.PlusIcon, Styles.Button))
                {
                    values.arraySize++;
                }

                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel + 1;

                for (int i = 0; i < values.arraySize; i++)
                {
                    DrawValueProperty(ref position, values, conflict, i);
                }

                DeleteOutstandingElement(values);
                conflict.Clear();
                CheckAndSaveConflict(values, conflict);
                EditorGUI.indentLevel = indentLevel;
            }
            EditorGUI.EndProperty();
        }

        private void DrawValueProperty(ref Rect position, SerializedProperty values, SerializableConflict conflict, int i)
        {
            SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");

            Rect r = Utilities.GetNextPropertyRect(ref position, value);
            Rect valueRect = new Rect(r.xMin + 5, r.yMin, position.width - EditorGUIUtility.singleLineHeight - 10f, EditorGUI.GetPropertyHeight(value));
            Rect trashRect = new Rect(valueRect.xMax + 5, r.yMin, EditorGUIUtility.singleLineHeight, r.height);

            EditorGUI.PropertyField(valueRect, value, GUIContent.none, value.isArray);

            if (GUI.Button(trashRect, Styles.TrashIcon, Styles.Button))
            {
                ElementToRemoveIndex = i;
            }

            DrawConflictIcon(r, i, conflict);
        }


    }
}
