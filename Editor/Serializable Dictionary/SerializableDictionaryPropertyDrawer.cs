using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace BatteryAcid.Serializables.Editor
{

    [CustomPropertyDrawer(typeof(SerializableDictionaryBase), true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
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

        private Dictionary<SerializableDictionaryPropertyId, SerializableDictionaryConflict> ConflictDictionary = new Dictionary<SerializableDictionaryPropertyId, SerializableDictionaryConflict>();
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

        public SerializableDictionaryPropertyDrawer()
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
                SerializedProperty keys = property.FindPropertyRelative("keys");
                SerializedProperty values = property.FindPropertyRelative("values");

                for (int i = 0; i < keys.arraySize; i++)
                {
                    SerializedProperty key = keys.GetArrayElementAtIndex(i);
                    SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");

                    float keyHeight = EditorGUI.GetPropertyHeight(key);
                    float valueHeight = EditorGUI.GetPropertyHeight(value);
                    float lineHeight = Mathf.Max(keyHeight, valueHeight);
                    totalHeight += lineHeight;
                }
                totalHeight += 5;

                SerializableDictionaryConflict conflict = GetConflict(property);
                if (conflict.Index != -1)
                {
                    totalHeight += conflict.LineHeight;
                }
            }
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keys = property.FindPropertyRelative("keys");
            SerializedProperty values = property.FindPropertyRelative("values");

            bool hasEnumKeys = keys.arraySize != 0 && keys.GetArrayElementAtIndex(0).propertyType == SerializedPropertyType.Enum;

            //Check for any conflicting keys if we have a conflict
            //then add it back into the serialized arrays.
            SerializableDictionaryConflict conflict = GetConflict(property);
            InsertConflict(keys, values, conflict);

            label = EditorGUI.BeginProperty(position, label, property);

            Rect foldout = GetNextPropertyRect(ref position);
            if (property.isExpanded = EditorGUI.Foldout(foldout, property.isExpanded, label))
            {
                Rect plusRect = new Rect(foldout.x + (foldout.width - EditorGUIUtility.singleLineHeight), foldout.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                if (!hasEnumKeys && GUI.Button(plusRect, Styles.PlusIcon, Styles.Button))
                {
                    keys.arraySize++;
                    values.arraySize = keys.arraySize;
                }

                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel + 1;

                for (int i = 0; i < keys.arraySize; i++)
                {
                    DrawKeyValuePair(ref position, keys, values, hasEnumKeys, conflict, i);
                }

                DeleteOutstandingElement(keys, values);
                conflict.Clear();
                CheckAndSaveConflict(keys, values, conflict);
                EditorGUI.indentLevel = indentLevel;
            }
            EditorGUI.EndProperty();
        }

        private void CheckAndSaveConflict(SerializedProperty keys, SerializedProperty values, SerializableDictionaryConflict conflict)
        {
            int count = keys.arraySize;
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

                for (int j = i + 1; j < count; j++)
                {
                    SerializedProperty key2 = keys.GetArrayElementAtIndex(j);
                    object key2Value = GetPropertyValue(key2);

                    if (ComparePropertyValues(key1Value, key2Value))
                    {
                        SerializedProperty value = values.GetArrayElementAtIndex(j);
                        SaveConflict(key2, value, j, i, conflict);
                        DeleteArrayElementAtIndex(keys, j);
                        DeleteArrayElementAtIndex(values, j);

                        return;
                    }
                }
            }
        }

        private void DeleteOutstandingElement(SerializedProperty keys, SerializedProperty values)
        {
            if (ElementToRemoveIndex != -1)
            {
                DeleteArrayElementAtIndex(values, ElementToRemoveIndex);
                DeleteArrayElementAtIndex(keys, ElementToRemoveIndex);
                ElementToRemoveIndex = -1;
            }
        }

        private void DrawValueProperty(Rect rect, SerializedProperty property)
        {
            if (!property.isArray)
            {
                EditorGUI.PropertyField(rect, property, GUIContent.none, false);
            }
            else
            {
                EditorGUI.PropertyField(rect, property, GUIContent.none, true);
            }
        }

        private void DrawKeyValuePair(ref Rect position, SerializedProperty keys, SerializedProperty values, bool hasEnumKeys, SerializableDictionaryConflict conflict, int i)
        {
            SerializedProperty key = keys.GetArrayElementAtIndex(i);
            SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");

            Rect r = GetNextPropertyRect(ref position, value);
            float w0 = EditorGUIUtility.labelWidth;
            float w1 = r.width - w0 - EditorGUIUtility.singleLineHeight;
            float indentOffset = value.isArray ? 0 : 13f; //Puts the value field in correct start location ... 🤦‍♂️

            Rect keyRect = new Rect(r.xMin + 5, r.yMin, w0 - 5, EditorGUI.GetPropertyHeight(key));

            Rect valueRect = hasEnumKeys ? new Rect(keyRect.xMax - indentOffset, r.yMin, w1 - 5 + EditorGUIUtility.singleLineHeight + indentOffset, EditorGUI.GetPropertyHeight(value)) : new Rect(keyRect.xMax - indentOffset, r.yMin, w1 - 5 + indentOffset, EditorGUI.GetPropertyHeight(value));
            Rect trashRect = new Rect(valueRect.xMax + 5, r.yMin, EditorGUIUtility.singleLineHeight, r.height);

            if (hasEnumKeys)
            {
                EditorGUI.LabelField(keyRect, key.enumDisplayNames[key.intValue]);
            }
            else
            {
                EditorGUI.PropertyField(keyRect, key, GUIContent.none, false);
            }

            DrawValueProperty(valueRect, value);

            if (!hasEnumKeys && GUI.Button(trashRect, Styles.TrashIcon, Styles.Button))
            {
                ElementToRemoveIndex = i;
            }

            DrawConflictIcon(r, i, conflict);
        }

        private static void DrawConflictIcon(Rect position, int index, SerializableDictionaryConflict conflict)
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

            iconPosition.size = size;
            GUI.Label(iconPosition, icon);
        }

        private void InsertConflict(SerializedProperty keys, SerializedProperty values, SerializableDictionaryConflict conflict)
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

        private Rect GetNextPropertyRect(ref Rect position, SerializedProperty property = null)
        {
            float height = property == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(property);
            Rect r = new Rect(position.xMin, position.yMin, position.width, height);
            float h = height + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }

        private SerializableDictionaryConflict GetConflict(SerializedProperty property)
        {
            SerializableDictionaryPropertyId propertyId = new SerializableDictionaryPropertyId(property);
            if (!ConflictDictionary.TryGetValue(propertyId, out SerializableDictionaryConflict conflict))
            {
                conflict = new SerializableDictionaryConflict();
                ConflictDictionary.Add(propertyId, conflict);
            }
            return conflict;
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

        private void SaveConflict(SerializedProperty key, SerializedProperty value, int index, int otherIndex, SerializableDictionaryConflict conflict)
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

        private void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index)
        {
            SerializedProperty property = arrayProperty.GetArrayElementAtIndex(index);
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = null;
            }
            arrayProperty.DeleteArrayElementAtIndex(index);
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
    }
}
