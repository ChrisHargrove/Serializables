using System.Text.RegularExpressions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    public class SerializableGuidField : VisualElement
    {
        public SerializableGuidField(SerializedProperty property)
        {
            GuidProperty = property;
            GuidStringProperty = GuidProperty.FindPropertyRelative("guidString");

            VisualElement topRow = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(true);

            TextField = new TextField(Regex.Match(property.propertyPath, "data\\[[0-9]\\]$").Success ? "" : property.displayName)
                .BindProp(GuidStringProperty)
                .FlexGrow(true)
                .SetReadOnly(true);

            LockButton = new Button(LockButtonToggle)
                .SetText("Unlock");

            topRow.Add(TextField);
            topRow.Add(LockButton);

            ButtonHolder = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(true)
                .Justify(Justify.SpaceAround)
                .Display(false);

            GenerateGuidButton = new Button(GenerateGuid)
                .SetText("Generate")
                .FlexGrow(true);
            ClearGuidButton = new Button(ClearGuid)
                .SetText("Clear")
                .FlexGrow(true);

            ButtonHolder.Add(GenerateGuidButton);
            ButtonHolder.Add(ClearGuidButton);

            this.Add(topRow);
            this.Add(ButtonHolder);
        }

        private SerializedProperty GuidProperty { get; }
        private SerializedProperty GuidStringProperty { get; }
        private bool IsLocked { get; set; } = true;
        private Button LockButton { get; }
        private Button GenerateGuidButton { get; }
        private Button ClearGuidButton { get; }
        private VisualElement ButtonHolder { get; }
        private TextField TextField { get; }

        private void LockButtonToggle()
        {
            IsLocked = !IsLocked;
            ButtonHolder.Display(IsLocked);
            LockButton.SetText(IsLocked ? "Unlock" : "Lock");
        }

        private void GenerateGuid()
        {
            TextField.value = Guid.NewGuid().ToString();
        }

        private void ClearGuid()
        {
            TextField.value = Guid.Empty.ToString();
        }
    }
}
