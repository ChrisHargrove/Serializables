using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SerializableDateTimeField(property);
        }
    }
}
