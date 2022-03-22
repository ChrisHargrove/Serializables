using UnityEditor;
using UnityEngine;

namespace BatteryAcid.Serializables.Editor
{
    public static class Styles
    {
        public static GUIContent PlusIcon = IconContent("Toolbar Plus", "Add a new item.");
        public static GUIContent TrashIcon = IconContent("TreeEditor.Trash", "Remove this item.");
        public static GUIContent WarningIcon = IconContent("console.warnicon.sml", "This key conflicts, therefore this entry will be removed!");
        public static GUIContent InfoIcon = IconContent("console.infoicon.sml", "Conflicting Key!");
        public static GUIContent NullIcon = IconContent("console.warnicon.sml", "Key is null so entry will be removed!");
        public static GUIContent LastPageIcon = IconContent("d_Animation.LastKey", "Go to last page");
        public static GUIContent NextPageIcon = IconContent("d_Animation.NextKey", "Go to next page");
        public static GUIContent PrevPageIcon = IconContent("d_Animation.PrevKey", "Go to previous page");
        public static GUIContent FirstPageIcon = IconContent("d_Animation.FirstKey", "Go to first page");

        public static GUIStyle Button = GUIStyle.none;
        public static GUIStyle PageCountLabel = EditorStyles.boldLabel;

        private static GUIContent IconContent(string iconName)
            => EditorGUIUtility.IconContent(iconName);

        private static GUIContent IconContent(string iconName, string tooltip)
            => EditorGUIUtility.IconContent(iconName, tooltip);
    }
}