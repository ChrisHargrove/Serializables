using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal static class VisualElementExtensions
    {
        public static T AddUssSheet<T>(this T element, StyleSheet styleSheet)
            where T : VisualElement
        {
            element.styleSheets.Add(styleSheet);
            return element;
        }

        public static T AddUssClass<T>(this T element, string className)
            where T : VisualElement
        {
            element.AddToClassList(className);
            return element;
        }

        public static T RemoveUssClass<T>(this T element, string className)
            where T : VisualElement
        {
            element.RemoveFromClassList(className);
            return element;
        }

        public static T AddChild<T>(this T element, VisualElement child)
            where T : VisualElement
        {
            element.Add(child);
            return element;
        }

        public static T FlexGrow<T>(this T element, bool shouldGrow)
            where T : VisualElement
        {
            element.style.flexGrow = new StyleFloat(shouldGrow ? 1 : 0);
            return element;
        }

        public static T FlexDirection<T>(this T element, FlexDirection direction)
            where T : VisualElement
        {
            element.style.flexDirection = new StyleEnum<FlexDirection>(direction);
            return element;
        }

        public static T Justify<T>(this T element, Justify justify)
            where T : VisualElement
        {
            element.style.justifyContent = new StyleEnum<Justify>(justify);
            return element;
        }

        public static T AlignItems<T>(this T element, Align align)
            where T : VisualElement
        {
            element.style.alignItems = new StyleEnum<Align>(align);
            return element;
        }

        public static T Display<T>(this T element, bool isDisplaying)
            where T : VisualElement
        {
            element.style.display = new StyleEnum<DisplayStyle>(isDisplaying ? DisplayStyle.Flex : DisplayStyle.None);
            return element;
        }

        public static T SetMinHeight<T>(this T element, float minHeight)
            where T : VisualElement
        {
            element.style.minHeight = new StyleLength(minHeight);
            return element;
        }

        public static T SetMaxHeight<T>(this T element, float maxHeight)
            where T : VisualElement
        {
            element.style.maxHeight = new StyleLength(maxHeight);
            return element;
        }

        public static T SetMinWidth<T>(this T element, float minWidth)
            where T : VisualElement
        {
            element.style.minWidth = new StyleLength(minWidth);
            return element;
        }

        public static T SetMaxWidth<T>(this T element, float maxWidth)
            where T : VisualElement
        {
            element.style.maxWidth = new StyleLength(maxWidth);
            return element;
        }

        public static T SetPadding<T>(this T element, int top, int right, int bottom, int left)
            where T : VisualElement
        {
            element.style.paddingTop = new StyleLength(top);
            element.style.paddingRight = new StyleLength(right);
            element.style.paddingBottom = new StyleLength(bottom);
            element.style.paddingLeft = new StyleLength(left);
            return element;
        }

        public static T SetMargin<T>(this T element, int top, int right, int bottom, int left)
            where T : VisualElement
        {
            element.style.marginTop = new StyleLength(top);
            element.style.marginRight = new StyleLength(right);
            element.style.marginBottom = new StyleLength(bottom);
            element.style.marginLeft = new StyleLength(left);
            return element;
        }

        public static T SetPaddingLeft<T>(this T element, int left)
            where T : VisualElement
        {
            element.style.paddingLeft = new StyleLength(left);
            return element;
        }

        public static T SetBorderColor<T>(this T element, Color color)
            where T : VisualElement
            => element.SetBorderColor(color, color, color, color);

        public static T SetBorderColor<T>(this T element, Color top, Color right, Color bottom, Color left)
            where T : VisualElement
        {
            element.style.borderLeftColor = new StyleColor(left);
            element.style.borderTopColor = new StyleColor(top);
            element.style.borderRightColor = new StyleColor(right);
            element.style.borderBottomColor = new StyleColor(bottom);
            return element;
        }

        public static T SetBorderRadius<T>(this T element, float radius)
            where T : VisualElement
        {
            element.style.borderTopLeftRadius = new StyleLength(radius);
            element.style.borderTopRightRadius = new StyleLength(radius);
            element.style.borderBottomRightRadius = new StyleLength(radius);
            element.style.borderBottomLeftRadius = new StyleLength(radius);
            return element;
        }

        public static T SetBackgroundColor<T>(this T element, Color color)
            where T : VisualElement
        {
            element.style.backgroundColor = new StyleColor(color);
            return element;
        }

        public static T BindProp<T>(this T element, SerializedProperty property)
            where T : IBindable
        {
            element.bindingPath = property.propertyPath;
            element.BindProperty(property);
            return element;
        }

        public static T SetText<T>(this T element, string text)
            where T : TextElement
        {
            element.text = text;
            return element;
        }

        public static T SetReadOnly<T>(this T element, bool isReadOnly)
            where T : TextInputBaseField<string>
        {
            element.isReadOnly = isReadOnly;
            return element;
        }

        public static T SetValue<T>(this T element, string value)
            where T : TextInputBaseField<string>
        {
            element.value = value;
            return element;
        }

        public static T SetDelayed<T>(this T element, bool isDelayed)
            where T : TextInputBaseField<string>
        {
            element.isDelayed = isDelayed;
            return element;
        }

        public static T SetOnValueChanged<T, V>(this T element, Action<V> onValueChanged)
            where T : INotifyValueChanged<V>
        {
            element.RegisterValueChangedCallback((evt) => onValueChanged?.Invoke(evt.newValue));
            return element;
        }

        public static T SetOnGeometryChanged<T>(this T element, Action<GeometryChangedEvent> onGeometryChanged)
            where T : VisualElement
        {
            element.RegisterCallback<GeometryChangedEvent>((evt) => onGeometryChanged?.Invoke(evt));
            return element;
        }

        public static T SetLabel<T>(this T element, string label = "")
            where T : PropertyField
        {
            element.label = label;
            return element;
        }

        public static T SetOnClicked<T>(this T element, Action onClicked)
            where T : Button
        {
            element.clicked += onClicked;
            return element;
        }

        public static T SetTooltip<T>(this T element, string tooltip)
            where T : VisualElement
        {
            element.tooltip = tooltip;
            return element;
        }

        public static T SetName<T>(this T element, string name)
            where T : VisualElement
        {
            element.name = name;
            return element;
        }

        public static T AssignTo<T>(this T element, ref T field)
            where T : VisualElement
        {
            field = element;
            return element;
        }

        public static Foldout SetText(this Foldout element, string text)
        {
            element.text = text;
            return element;
        }

        public static T OnAttachToPanel<T>(this T element, EventCallback<AttachToPanelEvent> callback)
            where T : VisualElement
        {
            element.RegisterCallback<AttachToPanelEvent>(callback);
            return element;
        }
    }
}