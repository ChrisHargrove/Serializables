using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BatteryAcid.Serializables.Editor
{
    public class Utilities
    {
        /// <summary>
        /// Mapping of SerializedPropertyTypes to their string representation in the SerializedProperty class.
        /// </summary>
        /// <typeparam name="SerializedPropertyType"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public static readonly Dictionary<SerializedPropertyType, string> PropertyTypeNames = new Dictionary<SerializedPropertyType, string>()
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

        /// <summary>
        /// Generated reflection propertyinfo data for each serialized property type.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<SerializedPropertyType, PropertyInfo> GenerateSerializedPropertyInfo()
        {
            Type serializedPropertyType = typeof(SerializedProperty);
            Dictionary<SerializedPropertyType, PropertyInfo> propertyTypeInfos = new Dictionary<SerializedPropertyType, PropertyInfo>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            foreach (KeyValuePair<SerializedPropertyType, string> kvp in Utilities.PropertyTypeNames)
            {
                PropertyInfo propertyInfo = serializedPropertyType.GetProperty(kvp.Value, flags);
                propertyTypeInfos.Add(kvp.Key, propertyInfo);
            }
            return propertyTypeInfos;
        }

        /// <summary>
        /// Utility method that takes the current rect position for the property drawer and increments its height by 1 row
        /// and returns the current property rect. If a property is passed in as an argument it will drop the rect
        /// by the height of that property.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Rect GetNextPropertyRect(ref Rect position, SerializedProperty property = null)
        {
            float height = property == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(property);
            Rect r = new Rect(position.xMin, position.yMin, position.width, height);
            float h = height + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }
    }
}
