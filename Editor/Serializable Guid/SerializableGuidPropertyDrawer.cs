using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using BatteryAcid.Serializables;
using UnityEngine;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    internal sealed class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SerializableGuidField(property);
        }
    }
}
