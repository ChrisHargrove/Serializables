using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    public class FallbackField : VisualElement
    {
        public FallbackField()
        {
            StyleSheet serializableStyles = AssetDatabaseExtensions.LoadFirstAsset<StyleSheet>("SerializableStyles");
            this.AddUssSheet(serializableStyles)
                .AddChild(new Foldout()
                    .AssignTo(ref foldout));

            this.OnAttachToPanel(OnAttachToPanel);
        }

        public FallbackField(SerializedProperty property) : this()
        {
            Data = property;
        }

        private SerializedProperty Data { get; }

        private readonly Foldout foldout;

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            if (this.parent.parent is ReorderableWrapper || this.parent.parent.name == "Content")
            {
                this.parent.SetMargin(0, 2, 0, 0);
                this.AddUssClass("fallback-field").AddUssClass("fallback-field-maximised");
                foldout.AddUssClass("foldout");

                foldout.SetOnValueChanged<Foldout, bool>(OnToggled);
            }

            this.Bind(Data.serializedObject);
        }

        private void OnToggled(bool isToggled)
        {
            if (this.parent.parent is ReorderableWrapper || this.parent.parent.name == "Content")
            {
                if (isToggled)
                {
                    this.AddUssClass("fallback-field-maximised");
                    return;
                }
                this.RemoveUssClass("fallback-field-maximised");
            }
        }

        public FallbackField Build()
        {
            bool isArrayElement = Regex.Match(Data.propertyPath, "data\\[[0-9]\\]\\.Value$").Success;

            foldout.SetText(isArrayElement ? "" : Data.displayName)
                .SetMargin(0, 0, 0, isArrayElement ? 10 : 0);

            SerializedProperty iterator = Data.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            string parentPropertyPath = iterator.propertyPath;
            iterator.Next(true);
            do
            {
                if (SerializedProperty.EqualContents(iterator, endProperty))
                    break;

                if (!iterator.propertyPath.StartsWith(parentPropertyPath))
                    break;

                foldout.AddChild(new PropertyField(iterator));
            }
            while (iterator.Next(false));
            return this;
        }
    }
}