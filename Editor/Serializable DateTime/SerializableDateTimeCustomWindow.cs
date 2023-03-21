using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal class SerializableDateTimeCustomWindow : UnityEditor.EditorWindow
    {
        public void OnEnable()
        {
            VisualElement container = new VisualElement();

            Year = new IntegerField("Year")
                .FlexGrow(true);
            Month = new IntegerField("Month")
                .FlexGrow(true);
            Day = new IntegerField("Day")
                .FlexGrow(true);

            Hour = new IntegerField("Hour")
                .FlexGrow(true);
            Minute = new IntegerField("Minute")
                .FlexGrow(true);
            Second = new IntegerField("Second")
                .FlexGrow(true);

            Kind = new EnumField("Kind", DateTimeKind.Local)
                .FlexGrow(true);

            VisualElement buttonHolder = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(true)
                .Justify(Justify.SpaceAround);

            Button confirmButton = new Button(ConfirmDateTime)
                .SetText("Confirm")
                .FlexGrow(true)
                .SetMinHeight(EditorGUIUtility.singleLineHeight);

            Button cancelButton = new Button(Cancel)
                .SetText("Cancel")
                .FlexGrow(true)
                .SetMinHeight(EditorGUIUtility.singleLineHeight);

            buttonHolder.Add(confirmButton);
            buttonHolder.Add(cancelButton);

            container.Add(Year);
            container.Add(Month);
            container.Add(Day);
            container.Add(Hour);
            container.Add(Minute);
            container.Add(Second);
            container.Add(Kind);
            container.Add(buttonHolder);

            rootVisualElement.Add(container);
        }

        private IntegerField Year { get; set; }
        private IntegerField Month { get; set; }
        private IntegerField Day { get; set; }
        private IntegerField Hour { get; set; }
        private IntegerField Minute { get; set; }
        private IntegerField Second { get; set; }
        private EnumField Kind { get; set; }

        public Action<DateTime> OnConfirmation { get; set; }

        private void ConfirmDateTime()
        {
            if (ValidateFields())
            {
                DateTime newDateTime = new DateTime(Year.value, Month.value, Day.value, Hour.value, Minute.value, Second.value, (DateTimeKind)Kind.value);
                OnConfirmation?.Invoke(newDateTime);
                this.Close();
            }
        }

        private void Cancel()
        {
            this.Close();
        }

        private bool ValidateFields()
        {
            //TODO: Validate datetime fields to stop incorrect dates being created.
            return true;
        }
    }
}