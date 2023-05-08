using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableStackBase), true)]
    public class SerializableStackPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty Values { get; set; }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Values = property.FindPropertyRelative("values");
            return new ReorderableList(Values)
                .SetTitle(property.displayName)
                .SetReorderable()
                .SetOnAddElement(OnAddElement)
                .SetOnCreateElement(OnCreateElement)
                .SetOnClearElements(OnClearElements)
                .EnableAddRemoveElements()
                .Build();
        }

        private VisualElement OnAddElement()
        {
            Values.arraySize++;
            Values.serializedObject.ApplyModifiedProperties();

            SerializedProperty valueProperty = Values.GetArrayElementAtIndex(Values.arraySize - 1).FindPropertyRelative("Value");
            return new PropertyField(valueProperty)
                .SetLabel()
                .BindProp(valueProperty);
        }

        private VisualElement OnCreateElement(SerializedProperty property)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            return new PropertyField(valueProperty)
                .SetLabel();
        }

        private void OnClearElements(VisualElement container, IEnumerable<VisualElement> elements)
        {
            Values.arraySize = 0;
            Values.serializedObject.ApplyModifiedProperties();
            container.Clear();
        }
    }
}
