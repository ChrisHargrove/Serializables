using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableUri))]
    public class SerializableUriPropertyDrawer : PropertyDrawer
    {
        private string Scheme { get; set; }
        private string Authority { get; set; }
        private string Path { get; set; }
        private string Query { get; set; }
        private string Fragment { get; set; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            int availableParts = 0;
            if (property.isExpanded)
            {
                if (!string.IsNullOrEmpty(Scheme)) availableParts++;
                if (!string.IsNullOrEmpty(Authority)) availableParts++;
                if (!string.IsNullOrEmpty(Path)) availableParts++;
                if (!string.IsNullOrEmpty(Query)) availableParts++;
                if (!string.IsNullOrEmpty(Fragment)) availableParts++;
            }
            return totalHeight + (EditorGUIUtility.singleLineHeight * availableParts);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty uriString = property.FindPropertyRelative("uriString");
            ExtractUriParts(uriString.stringValue);

            label = EditorGUI.BeginProperty(position, label, property);
            Rect uriRect = new Rect(position.xMin + EditorGUIUtility.labelWidth + 2f, position.yMin, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect foldout = GetNextPropertyRect(ref position);
            int indentLevel = EditorGUI.indentLevel;
            if (property.isExpanded = EditorGUI.Foldout(foldout, property.isExpanded, label))
            {
                GUI.enabled = false;
                EditorGUI.indentLevel = indentLevel + 1;
                if (!string.IsNullOrEmpty(Scheme))
                {
                    EditorGUI.DelayedTextField(GetNextPropertyRect(ref position), "Scheme", Scheme);
                }
                if (!string.IsNullOrEmpty(Authority))
                {
                    EditorGUI.DelayedTextField(GetNextPropertyRect(ref position), "Authority", Authority);
                }
                if (!string.IsNullOrEmpty(Path))
                {
                    EditorGUI.DelayedTextField(GetNextPropertyRect(ref position), "Path", Path);
                }
                if (!string.IsNullOrEmpty(Query))
                {
                    EditorGUI.DelayedTextField(GetNextPropertyRect(ref position), "Query", Query);
                }
                if (!string.IsNullOrEmpty(Fragment))
                {
                    EditorGUI.DelayedTextField(GetNextPropertyRect(ref position), "Fragment", Fragment);
                }
                GUI.enabled = true;

            }
            EditorGUI.indentLevel = indentLevel;
            string finalUri = EditorGUI.DelayedTextField(uriRect, "", uriString.stringValue);
            if (Uri.IsWellFormedUriString(finalUri, UriKind.RelativeOrAbsolute))
            {
                uriString.stringValue = finalUri;
            }
            else
            {
                Debug.LogError("Failed to create Uri, incorrect Uri String.");
            }
            EditorGUI.EndProperty();
        }

        private void ExtractUriParts(string uriString)
        {
            Uri uri = new Uri(uriString);
            Scheme = uri.Scheme;
            Authority = uri.Authority;
            Path = uri.AbsolutePath;
            Query = uri.Query;
            Fragment = uri.Fragment;
        }

        private Rect GetNextPropertyRect(ref Rect position, SerializedProperty property = null)
        {
            float height = property == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(property);
            Rect r = new Rect(position.xMin, position.yMin, position.width, height);
            float h = height + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }
    }
}
