using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal static class VisualElementExtensions
    {
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

        public static T Display<T>(this T element, bool isDisplaying)
            where T : VisualElement
        {
            element.style.display = new StyleEnum<DisplayStyle>(isDisplaying ? DisplayStyle.Flex : DisplayStyle.None);
            return element;
        }

        public static T BindProp<T>(this T element, SerializedProperty property)
            where T : IBindable
        {
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
    }
}