using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace BatteryAcid.Serializables.Editor
{
    public class AbstractSerializableConflictablePropertyDrawer<TConflictType> : PropertyDrawer
        where TConflictType : SerializableConflict, new()
    {
        protected Dictionary<SerializablePropertyId, TConflictType> ConflictDictionary = new Dictionary<SerializablePropertyId, TConflictType>();
        protected Dictionary<SerializedPropertyType, PropertyInfo> PropertyTypeInfos = Utilities.GenerateSerializedPropertyInfo();

        protected int ElementToRemoveIndex { get; set; } = -1;

        protected static void DrawConflictIcon(Rect position, int index, TConflictType conflict)
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

        protected TConflictType GetConflict(SerializedProperty property)
        {
            SerializablePropertyId propertyId = new SerializablePropertyId(property);
            if (!ConflictDictionary.TryGetValue(propertyId, out TConflictType conflict))
            {
                conflict = new TConflictType();
                ConflictDictionary.Add(propertyId, conflict);
            }
            return conflict;
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

        protected void SetPropertyValue(SerializedProperty property, object value)
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

        protected void SetPropertyValueArray(SerializedProperty property, object value)
        {
            object[] array = (object[])value;
            property.arraySize = array.Length;
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                SetPropertyValue(item, array[i]);
            }
        }

        protected void SetPropertyValueGeneric(SerializedProperty property, object value)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)value;
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextSiblingProperty = property.Copy();
            nextSiblingProperty.Next(false);

            while (currentProperty.Next(true) && currentProperty.propertyPath != nextSiblingProperty.propertyPath)
            {
                string name = currentProperty.propertyPath;
                SetPropertyValue(currentProperty, dict[name]);
            }
        }

        protected object GetPropertyValue(SerializedProperty property)
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

        protected object GetPropertyValueArray(SerializedProperty property)
        {
            object[] array = new object[property.arraySize];
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                array[i] = GetPropertyValue(item);
            }
            return array;
        }

        protected object GetPropertyValueGeneric(SerializedProperty property)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextSiblingProperty = property.Copy();
            nextSiblingProperty.Next(false);

            while (currentProperty.Next(true) && currentProperty.propertyPath != nextSiblingProperty.propertyPath)
            {
                string name = currentProperty.propertyPath;
                object value = GetPropertyValue(currentProperty);
                dict.Add(name, value);
            }
            return dict;
        }

        protected void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index)
        {
            SerializedProperty property = arrayProperty.GetArrayElementAtIndex(index);
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = null;
            }
            arrayProperty.DeleteArrayElementAtIndex(index);
        }
    }
}