using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal class SerializableDateTimeCustomWindow : UnityEditor.EditorWindow
    {
        private const string InputQuery = "unity-text-input";
        private Color DefaultTopColor => new Color(0.05098039f, 0.05098039f, 0.05098039f);
        private Color DefaultLeftColor => new Color(0.1294118f, 0.1294118f, 0.1294118f);
        private Color DefaultRightColor => new Color(0.1294118f, 0.1294118f, 0.1294118f);
        private Color DefaultBottomColor => new Color(0.1294118f, 0.1294118f, 0.1294118f);

        public void OnEnable()
        {
            VisualElement container = new VisualElement();

            Year = new IntegerField("Year")
                .FlexGrow(true);
            Month = new EnumField("Month", Months.January)
                .FlexGrow(true)
                .SetOnValueChanged((Enum newValue) => ValidateFields());
            Day = new IntegerField("Day")
                .FlexGrow(true)
                .SetOnValueChanged((int newValue) => ValidateFields());
            DayInput = Day.Q<VisualElement>(InputQuery);

            Hour = new IntegerField("Hour")
                .FlexGrow(true)
                .SetOnValueChanged((int newValue) => ValidateFields());
            HourInput = Hour.Q<VisualElement>(InputQuery);

            Minute = new IntegerField("Minute")
                .FlexGrow(true)
                .SetOnValueChanged((int newValue) => ValidateFields());
            MinuteInput = Minute.Q<VisualElement>(InputQuery);

            Second = new IntegerField("Second")
                .FlexGrow(true)
                .SetOnValueChanged((int newValue) => ValidateFields());
            SecondInput = Second.Q<VisualElement>(InputQuery);

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
        private EnumField Month { get; set; }
        private IntegerField Day { get; set; }
        private VisualElement DayInput { get; set; }
        private IntegerField Hour { get; set; }
        private VisualElement HourInput { get; set; }
        private IntegerField Minute { get; set; }
        private VisualElement MinuteInput { get; set; }
        private IntegerField Second { get; set; }
        private VisualElement SecondInput { get; set; }
        private EnumField Kind { get; set; }

        public Action<DateTime> OnConfirmation { get; set; }

        private void ConfirmDateTime()
        {
            if (ValidateFields())
            {
                DateTime newDateTime = new DateTime(Year.value, (int)(Months)Month.value, Day.value, Hour.value, Minute.value, Second.value, (DateTimeKind)Kind.value);
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
            bool isValid = true;
            int allowedDays = 0;
            allowedDays = (Months)Month.value switch
            {
                Months.February => 28,
                Months.April or Months.June or Months.September or Months.November => 30,
                _ => 31,
            };

            void validateField(IntegerField valueField, VisualElement borderField, int min, int max)
            {
                if (valueField.value < min || valueField.value > max)
                {
                    borderField.SetBorderColor(Color.red);
                    isValid = false;
                }
                else
                {
                    borderField.SetBorderColor(DefaultTopColor, DefaultRightColor, DefaultBottomColor, DefaultLeftColor);
                }
            }

            validateField(Day, DayInput, 1, allowedDays);
            validateField(Hour, HourInput, 0, 23);
            validateField(Minute, MinuteInput, 0, 59);
            validateField(Second, SecondInput, 0, 59);

            return isValid;
        }
    }
}