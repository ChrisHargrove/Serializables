using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(System.Object), true)]
    public class FallbackPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new FallbackField(property).Build();
        }
    }
}