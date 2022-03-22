using System.Collections.Generic;
using UnityEditor;

namespace BatteryAcid.Serializables.Editor
{
    public class SerializableHashSetConflictPropertyDrawer : AbstractSerializableConflictablePropertyDrawer<SerializableConflict>
    {
        protected void CheckAndSaveConflict(SerializedProperty values, SerializableConflict conflict)
        {
            int count = values.arraySize;
            List<object> indexRetrieving = new List<object>();
            HashSet<object> conflictSet = new HashSet<object>();
            for (int i = 0; i < count; i++)
            {
                SerializedProperty value1 = values.GetArrayElementAtIndex(i);
                object value1value = GetPropertyValue(value1.FindPropertyRelative("Value"));

                if (value1value == null)
                {
                    SaveConflict(value1, i, -1, conflict);
                    DeleteArrayElementAtIndex(values, i);
                    break;
                }

                if (!conflictSet.Add(value1value))
                {
                    SaveConflict(value1, i, indexRetrieving.IndexOf(i), conflict);
                    DeleteArrayElementAtIndex(values, i);
                    break;
                }
                indexRetrieving.Add(value1value);
            }
        }

        protected void SaveConflict(SerializedProperty value, int index, int otherIndex, SerializableConflict conflict)
        {
            conflict.Value = GetPropertyValue(value);
            conflict.LineHeight = (float)EditorGUI.GetPropertyHeight(value);
            conflict.Index = index;
            conflict.OtherIndex = otherIndex;
            conflict.IsValueExpanded = value.isExpanded;
        }

        protected void InsertConflict(SerializedProperty values, SerializableConflict conflict)
        {
            if (conflict.Index != -1)
            {
                values.InsertArrayElementAtIndex(conflict.Index);
                SerializedProperty value = values.GetArrayElementAtIndex(conflict.Index);
                SetPropertyValue(value, conflict.Value);
                value.isExpanded = conflict.IsValueExpanded;
            }
        }

        protected void DeleteOutstandingElement(SerializedProperty values)
        {
            if (ElementToRemoveIndex != -1)
            {
                DeleteArrayElementAtIndex(values, ElementToRemoveIndex);
                ElementToRemoveIndex = -1;
            }
        }
    }
}