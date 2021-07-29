using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableHashSetBase), true)]
    public class SerializableHashSetPropertyDrawer : PropertyDrawer
    {
        private static class Styles
        {
            public static GUIContent PlusIcon = IconContent("Toolbar Plus", "Add a new item.");
            public static GUIContent TrashIcon = IconContent("TreeEditor.Trash", "Remove this item.");
            public static GUIContent WarningIcon = IconContent("console.warnicon.sml", "This key conflicts, therefore this entry will be removed!");
            public static GUIContent InfoIcon = IconContent("console.infoicon.sml", "Conflicting Key!");
            public static GUIContent NullIcon = IconContent("console.warnicon.sml", "Key is null so entry will be removed!");

            public static GUIStyle Button = GUIStyle.none;

            private static GUIContent IconContent(string iconName)
                => EditorGUIUtility.IconContent(iconName);

            private static GUIContent IconContent(string iconName, string tooltip)
                => EditorGUIUtility.IconContent(iconName, tooltip);
        }

        private Dictionary<SerializablePropertyId, SerializableDictionaryConflict> ConflictDictionary = new Dictionary<SerializablePropertyId, SerializableDictionaryConflict>();
        private readonly Dictionary<SerializedPropertyType, string> PropertyTypeNames = new Dictionary<SerializedPropertyType, string>()
        {
            { SerializedPropertyType.Integer, "intValue" },
            { SerializedPropertyType.Boolean, "boolValue" },
            { SerializedPropertyType.Float, "floatValue" },
            { SerializedPropertyType.String, "stringValue" },
            { SerializedPropertyType.Color, "colorValue" },
            { SerializedPropertyType.ObjectReference, "objectReferenceValue" },
            { SerializedPropertyType.LayerMask, "intValue" },
            { SerializedPropertyType.Enum, "intValue" },
            { SerializedPropertyType.Vector2, "vector2Value" },
            { SerializedPropertyType.Vector3, "vector3Value" },
            { SerializedPropertyType.Vector4, "vector4Value" },
            { SerializedPropertyType.Rect, "rectValue" },
            { SerializedPropertyType.ArraySize, "intValue" },
            { SerializedPropertyType.Character, "intValue" },
            { SerializedPropertyType.AnimationCurve, "animationCurveValue" },
            { SerializedPropertyType.Bounds, "boundsValue" },
            { SerializedPropertyType.Quaternion, "quaternionValue" },
        };
        private Dictionary<SerializedPropertyType, PropertyInfo> PropertyTypeInfos;
        private int ElementToRemoveIndex { get; set; } = -1;

        public SerializableHashSetPropertyDrawer()
        {
            Type serializedPropertyType = typeof(SerializedProperty);
            PropertyTypeInfos = new Dictionary<SerializedPropertyType, PropertyInfo>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            foreach (KeyValuePair<SerializedPropertyType, string> kvp in PropertyTypeNames)
            {
                PropertyInfo propertyInfo = serializedPropertyType.GetProperty(kvp.Value, flags);
                PropertyTypeInfos.Add(kvp.Key, propertyInfo);
            }
        }

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
            Rect foldout = GetNextPropertyRect(ref position);
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

        private void CheckAndSaveConflict(SerializedProperty values, SerializableConflict conflict)
        {
            int count = values.arraySize;
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

                for (int j = i + 1; j < count; j++)
                {
                    SerializedProperty value2 = values.GetArrayElementAtIndex(j);
                    object value2value = GetPropertyValue(value2.FindPropertyRelative("Value"));

                    if (ComparePropertyValues(value1value, value2value))
                    {
                        SaveConflict(value2, j, i, conflict);
                        DeleteArrayElementAtIndex(values, j);
                        return;
                    }
                }
            }
        }

        private bool ComparePropertyValues(object value1, object value2)
        {
            if (value1 is Dictionary<string, object> dictionary1 && value2 is Dictionary<string, object> dictionary2)
                return CompareDictionaries(dictionary1, dictionary2);
            else
                return object.Equals(value1, value2);
        }

        private bool CompareDictionaries(Dictionary<string, object> dictionary1, Dictionary<string, object> dictionary2)
        {
            if (dictionary1.Count != dictionary2.Count)
                return false;

            foreach (KeyValuePair<string, object> kvp in dictionary1)
            {
                string key1 = kvp.Key;
                object value1 = kvp.Value;

                if (!dictionary2.TryGetValue(key1, out object value2))
                    return false;

                if (!ComparePropertyValues(value1, value2))
                    return false;
            }

            return true;
        }

        private void SaveConflict(SerializedProperty value, int index, int otherIndex, SerializableConflict conflict)
        {
            conflict.Value = GetPropertyValue(value);
            conflict.LineHeight = (float)EditorGUI.GetPropertyHeight(value);
            conflict.Index = index;
            conflict.OtherIndex = otherIndex;
            conflict.IsValueExpanded = value.isExpanded;
        }

        private Rect GetNextPropertyRect(ref Rect position, SerializedProperty property = null)
        {
            float height = property == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(property);
            Rect r = new Rect(position.xMin, position.yMin, position.width, height);
            float h = height + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }

        private void DeleteOutstandingElement(SerializedProperty values)
        {
            if (ElementToRemoveIndex != -1)
            {
                DeleteArrayElementAtIndex(values, ElementToRemoveIndex);
                ElementToRemoveIndex = -1;
            }
        }

        private void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index)
        {
            SerializedProperty property = arrayProperty.GetArrayElementAtIndex(index);
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = null;
            }
            arrayProperty.DeleteArrayElementAtIndex(index);
        }

        private void DrawValueProperty(ref Rect position, SerializedProperty values, SerializableConflict conflict, int i)
        {
            SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");

            Rect r = GetNextPropertyRect(ref position, value);
            Rect valueRect = new Rect(r.xMin + 5, r.yMin, position.width - EditorGUIUtility.singleLineHeight - 10f, EditorGUI.GetPropertyHeight(value));
            Rect trashRect = new Rect(valueRect.xMax + 5, r.yMin, EditorGUIUtility.singleLineHeight, r.height);

            EditorGUI.PropertyField(valueRect, value, GUIContent.none, value.isArray);

            if (GUI.Button(trashRect, Styles.TrashIcon, Styles.Button))
            {
                ElementToRemoveIndex = i;
            }

            DrawConflictIcon(r, i, conflict);
        }

        private static void DrawConflictIcon(Rect position, int index, SerializableConflict conflict)
        {
            Rect iconPosition = position;
            GUIContent icon = null;
            Vector2 size = Vector2.zero;

            if (index == conflict.Index && conflict.OtherIndex == -1)
            {
                size = Styles.Button.CalcSize(Styles.NullIcon);
                icon = Styles.NullIcon;
            }
            else if (index == conflict.Index)
            {
                size = Styles.Button.CalcSize(Styles.WarningIcon);
                icon = Styles.WarningIcon;
            }
            else if (index == conflict.OtherIndex)
            {
                size = Styles.Button.CalcSize(Styles.InfoIcon);
                icon = Styles.InfoIcon;
            }

            if (icon != null)
            {
                iconPosition.size = size;
                GUI.Label(iconPosition, icon);
            }
        }

        private SerializableConflict GetConflict(SerializedProperty property)
        {
            SerializablePropertyId propertyId = new SerializablePropertyId(property);
            if (!ConflictDictionary.TryGetValue(propertyId, out SerializableDictionaryConflict conflict))
            {
                conflict = new SerializableDictionaryConflict();
                ConflictDictionary.Add(propertyId, conflict);
            }
            return conflict;
        }

        private void InsertConflict(SerializedProperty values, SerializableConflict conflict)
        {
            if (conflict.Index != -1)
            {
                values.InsertArrayElementAtIndex(conflict.Index);
                SerializedProperty value = values.GetArrayElementAtIndex(conflict.Index);
                SetPropertyValue(value, conflict.Value);
                value.isExpanded = conflict.IsValueExpanded;
            }
        }

        private void SetPropertyValue(SerializedProperty property, object value)
        {
            if (PropertyTypeInfos.TryGetValue(property.propertyType, out PropertyInfo propertyInfo))
            {
                propertyInfo.SetValue(property, value, null);
            }
            else
            {
                if (property.isArray)
                    SetPropertyValueArray(property, value);
                else
                    SetPropertyValueGeneric(property, value);
            }
        }

        private void SetPropertyValueArray(SerializedProperty property, object value)
        {
            object[] array = (object[])value;
            property.arraySize = array.Length;
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                SetPropertyValue(item, array[i]);
            }
        }

        private void SetPropertyValueGeneric(SerializedProperty property, object value)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)value;
            SerializedProperty iterator = property.Copy();
            if (iterator.Next(true))
            {
                SerializedProperty end = property.GetEndProperty();
                do
                {
                    string name = iterator.name;
                    SetPropertyValue(iterator, dict[name]);
                } while (iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
        }

        private object GetPropertyValue(SerializedProperty property)
        {
            if (PropertyTypeInfos.TryGetValue(property.propertyType, out PropertyInfo propertyInfo))
            {
                return propertyInfo.GetValue(property, null);
            }
            else
            {
                if (property.isArray)
                    return GetPropertyValueArray(property);
                else
                    return GetPropertyValueGeneric(property);
            }
        }

        private object GetPropertyValueArray(SerializedProperty property)
        {
            object[] array = new object[property.arraySize];
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                array[i] = GetPropertyValue(item);
            }
            return array;
        }

        private object GetPropertyValueGeneric(SerializedProperty property)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            SerializedProperty iterator = property.Copy();
            if (iterator.Next(true))
            {
                SerializedProperty end = property.GetEndProperty();
                do
                {
                    string name = iterator.name;
                    object value = GetPropertyValue(iterator);
                    dict.Add(name, value);
                } while (iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
            return dict;
        }
    }
}
