using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal sealed class SerializableDateTimeField : VisualElement
    {
        public SerializableDateTimeField(SerializedProperty property)
        {
            DateTimeProperty = property;
            TicksProperty = DateTimeProperty.FindPropertyRelative("ticks");
            KindProperty = DateTimeProperty.FindPropertyRelative("kind");

            DateTime = new DateTime(TicksProperty.longValue, (DateTimeKind)KindProperty.intValue);

            TopRow = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(true)
                .SetOnGeometryChanged(OnDateTimeFieldGeometryChanged);

            TextField = new TextField(Regex.Match(property.propertyPath, "data\\[[0-9]\\]$").Success ? "" : property.displayName)
                .FlexGrow(true)
                .SetReadOnly(true)
                .SetValue(DateTime.ToString());

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

            UtcNow = new Button(() => GenerateDateTime(DateTimeKind.Utc))
                .SetText("UTC")
                .FlexGrow(true);
            Now = new Button(() => GenerateDateTime(DateTimeKind.Local))
                .SetText("Local")
                .FlexGrow(true);
            Custom = new Button(() => OpenCustomWindow())
                .SetText("Custom")
                .FlexGrow(true);

            ButtonHolder.Add(UtcNow);
            ButtonHolder.Add(Now);
            ButtonHolder.Add(Custom);

            this.Add(TopRow);
            this.Add(ButtonHolder);
        }

        private SerializedProperty DateTimeProperty { get; }
        private SerializedProperty TicksProperty { get; }
        private SerializedProperty KindProperty { get; }

        private DateTime DateTime { get; set; }

        private VisualElement TopRow { get; }
        private TextField TextField { get; }
        private Label TextFieldLabel { get; }
        private VisualElement TextFieldInput { get; }
        private VisualElement ButtonHolder { get; }
        private Button UtcNow { get; }
        private Button Now { get; }
        private Button Custom { get; }

        private bool IsLocked { get; set; } = true;
        private Button LockButton { get; }

        private void LockButtonToggle()
        {
            IsLocked = !IsLocked;
            ButtonHolder.Display(!IsLocked);
            LockButton.SetText(IsLocked ? "Unlock" : "Lock");
        }

        private void GenerateDateTime(DateTimeKind kind)
        {
            switch (kind)
            {
                case DateTimeKind.Utc:
                    DateTime = DateTime.UtcNow;
                    break;
                default:
                    DateTime = DateTime.Now;
                    break;
            }
            UpdateDateTimeProperties();
        }

        private void OpenCustomWindow()
        {
            SerializableDateTimeCustomWindow window = UnityEditor.EditorWindow.GetWindow<SerializableDateTimeCustomWindow>(true, "Custom DateTime", true);
            window.OnConfirmation = ConfirmCustomDateTime;
        }

        private void ConfirmCustomDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
            UpdateDateTimeProperties();
        }

        private void UpdateDateTimeProperties()
        {
            TicksProperty.longValue = DateTime.Ticks;
            KindProperty.intValue = (int)DateTime.Kind;
            DateTimeProperty.serializedObject.ApplyModifiedProperties();
            TextField.SetValue(DateTime.ToString());
        }

        private void OnLockButtonGeometryChanged(GeometryChangedEvent evt)
        {
            float maxWidth = TopRow.layout.width - (TextFieldLabel.layout.width + LockButton.layout.width + 10);
            TextFieldInput.SetMaxWidth(maxWidth);
        }

        private void OnDateTimeFieldGeometryChanged(GeometryChangedEvent evt)
        {
            float maxWidth = evt.newRect.width - (TextFieldLabel.layout.width + LockButton.layout.width + 10);
            TextFieldInput.SetMaxWidth(maxWidth);
        }
    }
}
