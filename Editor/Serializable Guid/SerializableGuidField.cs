using System.Text.RegularExpressions;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal sealed class SerializableGuidField : VisualElement
    {
        public SerializableGuidField(SerializedProperty property)
        {
            GuidProperty = property;
            GuidStringProperty = GuidProperty.FindPropertyRelative("guidString");

            TopRow = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(true)
                .SetOnGeometryChanged(OnGuidFieldGeometryChanged);

            TextField = new TextField(Regex.Match(property.propertyPath, "data\\[[0-9]\\]$").Success ? "" : property.displayName)
                .BindProp(GuidStringProperty)
                .FlexGrow(true)
                .SetReadOnly(true);

            TextFieldLabel = TextField.Q<Label>();
            TextFieldInput = TextField.Q("unity-text-input");

            LockButton = new Button(LockButtonToggle)
                .SetText("Unlock")
                .SetOnGeometryChanged(OnLockButtonGeometryChanged);

            TopRow.Add(TextField);
            TopRow.Add(LockButton);

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

            this.Add(TopRow);
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
        private Label TextFieldLabel { get; }
        private VisualElement TextFieldInput { get; }
        private VisualElement TopRow { get; }

        private void LockButtonToggle()
        {
            IsLocked = !IsLocked;
            ButtonHolder.Display(!IsLocked);
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

        private void OnLockButtonGeometryChanged(GeometryChangedEvent evt)
        {
            float maxWidth = TopRow.layout.width - (TextFieldLabel.layout.width + LockButton.layout.width + 10);
            TextFieldInput.SetMaxWidth(maxWidth);
        }

        private void OnGuidFieldGeometryChanged(GeometryChangedEvent evt)
        {
            float maxWidth = evt.newRect.width - (TextFieldLabel.layout.width + LockButton.layout.width + 10);
            TextFieldInput.SetMaxWidth(maxWidth);
        }
    }
}
