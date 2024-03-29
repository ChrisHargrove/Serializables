using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BatteryAcid.Serializables.Editor
{
    public class SerializableDictionaryConflictPropertyDrawer : AbstractSerializableConflictablePropertyDrawer<SerializableDictionaryConflict>
    {
        protected void CheckAndSaveConflict(SerializedProperty keys, SerializedProperty values, SerializableDictionaryConflict conflict)
        {
            int count = keys.arraySize;
            List<object> indexRetrieving = new List<object>();
            HashSet<object> conflictSet = new HashSet<object>();
            for (int i = 0; i < count; i++)
            {
                SerializedProperty key1 = keys.GetArrayElementAtIndex(i);
                object key1Value = GetPropertyValue(key1);

                if (key1 == null)
                {
                    SerializedProperty value = values.GetArrayElementAtIndex(i);
                    SaveConflict(key1, value, i, -1, conflict);
                    DeleteArrayElementAtIndex(values, i);
                    DeleteArrayElementAtIndex(keys, i);
                    break;
                }

                if (!conflictSet.Add(key1Value))
                {
                    // Already in the hash set.
                    SerializedProperty value = values.GetArrayElementAtIndex(i);
                    SaveConflict(key1, value, i, indexRetrieving.IndexOf(key1Value), conflict);
                    DeleteArrayElementAtIndex(values, i);
                    DeleteArrayElementAtIndex(keys, i);
                    break;
                }
                indexRetrieving.Add(key1Value);
            }
        }

        protected void SaveConflict(SerializedProperty key, SerializedProperty value, int index, int otherIndex, SerializableDictionaryConflict conflict)
        {
            conflict.Key = GetPropertyValue(key);
            conflict.Value = GetPropertyValue(value);
            float keyPropertyHeight = EditorGUI.GetPropertyHeight(key);
            float valuePropertyHeight = EditorGUI.GetPropertyHeight(value);
            float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
            conflict.LineHeight = lineHeight;
            conflict.Index = index;
            conflict.OtherIndex = otherIndex;
            conflict.IsKeyExpanded = key.isExpanded;
            conflict.IsValueExpanded = value.isExpanded;
        }

        protected void InsertConflict(SerializedProperty keys, SerializedProperty values, SerializableDictionaryConflict conflict)
        {
            if (conflict.Index != -1)
            {
                keys.InsertArrayElementAtIndex(conflict.Index);
                SerializedProperty key = keys.GetArrayElementAtIndex(conflict.Index);
                SetPropertyValue(key, conflict.Key);
                key.isExpanded = conflict.IsKeyExpanded;

                values.InsertArrayElementAtIndex(conflict.Index);
                SerializedProperty value = values.GetArrayElementAtIndex(conflict.Index);
                SetPropertyValue(value, conflict.Value);
                value.isExpanded = conflict.IsValueExpanded;
            }
        }

        protected void DeleteOutstandingElement(SerializedProperty keys, SerializedProperty values)
        {
            if (ElementToRemoveIndex != -1)
            {
                DeleteArrayElementAtIndex(values, ElementToRemoveIndex);
                DeleteArrayElementAtIndex(keys, ElementToRemoveIndex);
                ElementToRemoveIndex = -1;
            }
        }
    }
}