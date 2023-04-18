using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableStackBase), true)]
    public class SerializableStackPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty values = property.FindPropertyRelative("values");
            return new SerializableReorderableList(values)
                .SetTitle(property.displayName)
                .Build();
        }

    }
}
