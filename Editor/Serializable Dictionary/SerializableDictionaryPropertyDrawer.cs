using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BatteryAcid.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionaryBase), true)]
    public class SerializableDictionaryPropertyDrawer : SerializableDictionaryConflictPropertyDrawer
    {
        private const int MaxItemsPerPage = 25;
        private const int MessageBoxOffset = 10;

        private int? InitialElementCount { get; set; }
        private int CurrentPageIndex { get; set; }
        private int PageCount { get; set; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                SerializedProperty keys = property.FindPropertyRelative("keys");
                SerializedProperty values = property.FindPropertyRelative("values");

                SerializableDictionaryConflict conflict = GetConflict(property);

                int totalElementCount = keys.arraySize + (conflict.IsConflicting ? 1 : 0);
                int pageCount = GetPageCount(totalElementCount);
                int elementCount = GetPageElementCount(CurrentPageIndex, totalElementCount);
                int startIndex = GetPageElementStartIndex(CurrentPageIndex, totalElementCount);

                for (int i = startIndex; i < startIndex + elementCount + (conflict.IsConflicting ? -1 : 0); i++)
                {
                    SerializedProperty key = keys.GetArrayElementAtIndex(i);
                    SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");

                    float keyHeight = EditorGUI.GetPropertyHeight(key);
                    float valueHeight = EditorGUI.GetPropertyHeight(value);
                    float lineHeight = Mathf.Max(keyHeight, valueHeight);
                    totalHeight += lineHeight + 1;
                }
                totalHeight += 5;

                if (conflict.IsConflicting)
                {
                    if (elementCount != MaxItemsPerPage)
                    {
                        totalHeight += conflict.LineHeight;
                    }

                    string message = $"Conflict found! \n";
                    message += conflict.OtherIndex != -1 ? $"Newly Conflicting Index: {conflict.Index}\n" : $"Index is null: {conflict.Index}\n";
                    message += conflict.OtherIndex != -1 ? $"Original Conflicting Index: {conflict.OtherIndex}" : "";

                    Vector2 messageBoxSize = EditorStyles.label.CalcSize(new GUIContent(message));
                    totalHeight += messageBoxSize.y;
                }
            }
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keys = property.FindPropertyRelative("keys");
            SerializedProperty values = property.FindPropertyRelative("values");

            bool hasEnumKeys = keys.arraySize != 0 && keys.GetArrayElementAtIndex(0).propertyType == SerializedPropertyType.Enum;

            //Check for any conflicting keys if we have a conflict
            //then add it back into the serialized arrays.
            SerializableDictionaryConflict conflict = GetConflict(property);
            InsertConflict(keys, values, conflict);

            label = EditorGUI.BeginProperty(position, label, property);

            Rect foldout = Utilities.GetNextPropertyRect(ref position);
            if (property.isExpanded = EditorGUI.Foldout(foldout, property.isExpanded, label))
            {
                int totalElementCount = keys.arraySize;
                int pageCount = GetPageCount(totalElementCount);
                int elementCount = GetPageElementCount(CurrentPageIndex, totalElementCount);
                int startIndex = GetPageElementStartIndex(CurrentPageIndex, totalElementCount);

                GUI.enabled = !conflict.IsConflicting;
                Rect plusRect = new Rect(foldout.x + (foldout.width - EditorGUIUtility.singleLineHeight), foldout.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                if (!hasEnumKeys && GUI.Button(plusRect, Styles.PlusIcon, Styles.Button))
                {
                    keys.arraySize++;
                    values.arraySize = keys.arraySize;

                    CurrentPageIndex = GetPageCount(keys.arraySize) - 1;
                }
                GUI.enabled = true;

                DrawPageSelector(plusRect, keys);

                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel + 1;

                for (int i = startIndex; i < startIndex + elementCount; i++)
                {
                    GUI.enabled = !(conflict.IsConflicting && i != conflict.Index && i != conflict.OtherIndex);
                    DrawKeyValuePair(ref position, keys, values, hasEnumKeys, conflict, i);
                    GUI.enabled = true;
                }

                DeleteOutstandingElement(keys, values);
                conflict.Clear();
                CheckAndSaveConflict(keys, values, conflict);
                EditorGUI.indentLevel = indentLevel;
            }

            DrawConflictMessage(position, conflict);

            EditorGUI.EndProperty();
        }

        private void DrawConflictMessage(Rect rect, SerializableDictionaryConflict conflict)
        {
            if (!conflict.IsConflicting)
            {
                return;
            }

            string message = $"Conflict found! \n";
            message += conflict.OtherIndex != -1 ? $"Newly Conflicting Index: {conflict.Index}\n" : $"Index is null: {conflict.Index}\n";
            message += conflict.OtherIndex != -1 ? $"Original Conflicting Index: {conflict.OtherIndex}" : "";

            Vector2 messageBoxSize = EditorStyles.label.CalcSize(new GUIContent(message));

            Rect messageBoxRect = new Rect(rect.x, rect.y + MessageBoxOffset, rect.width, messageBoxSize.y);
            EditorGUI.HelpBox(messageBoxRect, message, MessageType.Warning);
        }

        private void DrawPageSelector(Rect rect, SerializedProperty keys)
        {
            float singleLine = EditorGUIUtility.singleLineHeight;
            int pageCount = GetPageCount(keys.arraySize);
            Rect lastArrowRect = new Rect(rect.x - singleLine - 5, rect.y, singleLine, singleLine);
            GUI.enabled = !((pageCount - 1) == CurrentPageIndex);
            if (GUI.Button(lastArrowRect, Styles.LastPageIcon, Styles.Button))
            {
                CurrentPageIndex = pageCount - 1;
            }
            GUI.enabled = true;
            Rect nextArrowRect = new Rect(lastArrowRect.x - singleLine, rect.y, singleLine, singleLine);
            GUI.enabled = !((pageCount - 1) == CurrentPageIndex);
            if (GUI.Button(nextArrowRect, Styles.NextPageIcon, Styles.Button))
            {
                CurrentPageIndex++;
            }
            GUI.enabled = true;
            Vector2 size = Styles.PageCountLabel.CalcSize(new GUIContent($" / {pageCount}  "));
            Rect pageCountLabelRect = new Rect(nextArrowRect.x - size.x, nextArrowRect.y, size.x, singleLine);
            EditorGUI.LabelField(pageCountLabelRect, $" / {pageCount}  ", Styles.PageCountLabel);
            Vector2 inputTextSize = Styles.PageCountLabel.CalcSize(new GUIContent($"{CurrentPageIndex}")) + new Vector2(30, 0);
            Rect pageIndexInputRect = new Rect(pageCountLabelRect.x - inputTextSize.x, pageCountLabelRect.y, inputTextSize.x, singleLine);

            Rect prevArrowRect = new Rect(pageIndexInputRect.x - singleLine - 5f, pageIndexInputRect.y, singleLine, singleLine);
            GUI.enabled = !((CurrentPageIndex - 1) < 0);
            if (GUI.Button(prevArrowRect, Styles.PrevPageIcon, Styles.Button))
            {
                CurrentPageIndex--;
            }
            GUI.enabled = true;
            Rect firstArrowRect = new Rect(prevArrowRect.x - singleLine, prevArrowRect.y, singleLine, singleLine);
            GUI.enabled = !((CurrentPageIndex - 1) < 0);
            if (GUI.Button(firstArrowRect, Styles.FirstPageIcon, Styles.Button))
            {
                CurrentPageIndex = 0;
            }
            GUI.enabled = true;
            //TODO: bug here need to check it out!
            int newPageIndex = EditorGUI.DelayedIntField(pageIndexInputRect, CurrentPageIndex + 1);
            if (newPageIndex > -1 && newPageIndex < pageCount - 1)
            {
                CurrentPageIndex = newPageIndex;
            }
        }

        private void DrawValueProperty(Rect rect, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property, GUIContent.none, property.isArray);
        }

        private void DrawKeyValuePair(ref Rect position, SerializedProperty keys, SerializedProperty values, bool hasEnumKeys, SerializableDictionaryConflict conflict, int i)
        {
            SerializedProperty key = keys.GetArrayElementAtIndex(i);
            SerializedProperty value = values.GetArrayElementAtIndex(i).FindPropertyRelative("Value");

            Rect r = Utilities.GetNextPropertyRect(ref position, value);
            float w0 = EditorGUIUtility.labelWidth;
            float w1 = r.width - w0 - EditorGUIUtility.singleLineHeight;
            float indentOffset = value.isArray ? 0 : 13f; //Puts the value field in correct start location ... 🤦‍♂️

            Rect keyRect = new Rect(r.xMin + 5, r.yMin, w0 - 5, EditorGUI.GetPropertyHeight(key));

            Rect valueRect = hasEnumKeys ? new Rect(keyRect.xMax - indentOffset, r.yMin, w1 - 5 + EditorGUIUtility.singleLineHeight + indentOffset, EditorGUI.GetPropertyHeight(value)) : new Rect(keyRect.xMax - indentOffset, r.yMin, w1 - 5 + indentOffset, EditorGUI.GetPropertyHeight(value));
            Rect trashRect = new Rect(valueRect.xMax + 5, r.yMin, EditorGUIUtility.singleLineHeight, r.height);

            if (hasEnumKeys)
            {
                EditorGUI.LabelField(keyRect, key.enumDisplayNames[key.intValue]);
            }
            else
            {
                EditorGUI.PropertyField(keyRect, key, GUIContent.none, false);
            }

            DrawValueProperty(valueRect, value);

            if (!hasEnumKeys && GUI.Button(trashRect, Styles.TrashIcon, Styles.Button))
            {
                ElementToRemoveIndex = i;
            }

            DrawConflictIcon(r, i, conflict);
        }

        #region Pagination Methods

        private int GetPageCount(int elementCount)
        {
            if (!InitialElementCount.HasValue || InitialElementCount.Value != elementCount)
            {
                InitialElementCount = elementCount;
                PageCount = (InitialElementCount.Value / MaxItemsPerPage) + (InitialElementCount.Value % MaxItemsPerPage > 0 ? 1 : 0);
                return PageCount;
            }
            return PageCount;
        }

        private int GetPageElementStartIndex(int pageIndex, int totalElementCount)
        {
            if (totalElementCount == 0)
            {
                return 0;
            }
            return MaxItemsPerPage * pageIndex;
        }

        private int GetPageElementCount(int pageIndex, int totalElementCount)
        {
            if (totalElementCount == 0)
            {
                return 0;
            }
            if ((PageCount - 1) == pageIndex)
            {
                //TODO: calculate last page element count
                return (totalElementCount % MaxItemsPerPage) != 0 ? totalElementCount % MaxItemsPerPage : MaxItemsPerPage;
            }
            return MaxItemsPerPage;
        }

        #endregion
    }
}
