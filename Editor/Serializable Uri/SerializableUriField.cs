using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal sealed class SerializableUriField : VisualElement
    {
        public SerializableUriField(SerializedProperty property)
        {
            UriProperty = property;
            UriStringProperty = property.FindPropertyRelative("uriString");

            Uri = new Uri(UriStringProperty.stringValue);

            TopRow = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(true)
                .SetOnGeometryChanged(OnTextFieldGeometryChanged);

            TextField = new TextField(Regex.Match(property.propertyPath, "data\\[[0-9]\\]$").Success ? "" : property.displayName)
                .FlexGrow(true)
                .BindProp(UriStringProperty)
                .SetDelayed(true)
                .SetOnValueChanged((string newUri) => UpdateFields(newUri));

            TextFieldLabel = TextField.Q<Label>();
            TextFieldInput = TextField.Q("unity-text-input");

            InfoToggle = new Button(ToggleInfo)
                .SetText("Info");

            TopRow.Add(TextField);
            TopRow.Add(InfoToggle);

            InfoHolder = new VisualElement()
                .FlexDirection(FlexDirection.Column)
                .FlexGrow(false)
                .Display(false)
                .SetPaddingLeft(20);

            SchemeField = new TextField("Scheme")
                .SetReadOnly(true)
                .SetValue(Uri.Scheme + Uri.SchemeDelimiter);
            AuthorityField = new TextField("Authority")
                .SetReadOnly(true)
                .SetValue(Uri.Authority);
            PathField = new TextField("Path")
                .SetReadOnly(true)
                .SetValue(Uri.AbsolutePath);
            QueryField = new TextField("Query")
                .SetReadOnly(true)
                .SetValue(Uri.Query);
            FragmentField = new TextField("Fragment")
                .SetReadOnly(true)
                .SetValue(Uri.Fragment);

            InfoHolder.Add(SchemeField);
            InfoHolder.Add(AuthorityField);
            InfoHolder.Add(PathField);
            InfoHolder.Add(QueryField);
            InfoHolder.Add(FragmentField);

            this.Add(TopRow);
            this.Add(InfoHolder);
        }

        private SerializedProperty UriProperty { get; }
        private SerializedProperty UriStringProperty { get; set; }

        private Uri Uri { get; set; }

        private VisualElement TopRow { get; }
        private TextField TextField { get; }
        private Label TextFieldLabel { get; }
        private VisualElement TextFieldInput { get; }
        private Button InfoToggle { get; }
        private VisualElement InfoHolder { get; }

        private TextField SchemeField { get; }
        private TextField AuthorityField { get; }
        private TextField PathField { get; }
        private TextField QueryField { get; }
        private TextField FragmentField { get; }

        private bool IsShowingInfo { get; set; }

        private void ToggleInfo()
        {
            IsShowingInfo = !IsShowingInfo;
            InfoHolder.Display(!IsShowingInfo);
        }

        private void UpdateFields(string newUri)
        {
            Uri = new Uri(newUri);
            SchemeField.SetValue(Uri.Scheme + Uri.SchemeDelimiter);
            AuthorityField.SetValue(Uri.Authority);
            PathField.SetValue(Uri.AbsolutePath);
            QueryField.SetValue(Uri.Query);
            FragmentField.SetValue(Uri.Fragment);
        }

        private void OnTextFieldGeometryChanged(GeometryChangedEvent evt)
        {
            float maxWidth = evt.newRect.width - (TextFieldLabel.layout.width + InfoToggle.layout.width + 10);
            TextFieldInput.SetMaxWidth(maxWidth);
        }
    }
}