using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    public class SerializableReorderableList : VisualElement
    {
        public SerializableReorderableList()
        {
            VisualTreeAsset uxml = AssetDatabaseExtensions.LoadFirstAsset<VisualTreeAsset>("SerializableReorderableList");
            uxml.CloneTree(this);

            Title = this.Q<Label>("Title");
            ControlBar = this.Q("ControlBar");
            Container = this.Q("ElementHolder");
            AddButton = this.Q<Button>("Add");
            RemoveButton = this.Q<Button>("Remove");
            ClearButton = this.Q<Button>("Clear");

            AddButton.clicked += AddElement;

            ClearButton.clicked += ClearElements;
        }

        public SerializableReorderableList(SerializedProperty property) : this()
        {
            Data = property;
        }

        private Label Title { get; set; }
        private VisualElement ControlBar { get; set; }
        private VisualElement Container { get; set; }
        private Button AddButton { get; set; }
        private Button RemoveButton { get; set; }
        private Button ClearButton { get; set; }

        private SerializedProperty Data { get; set; }

        private void AddElement()
        {
            Data.arraySize++;
            Data.serializedObject.ApplyModifiedProperties();

            SerializedProperty prop = Data.GetArrayElementAtIndex(Data.arraySize - 1).FindPropertyRelative("Value");
            PropertyField propField = new PropertyField()
                .BindProp(prop)
                .SetLabel();
            Container.Add(propField);
        }

        private void ClearElements()
        {
            Data.arraySize = 0;
            Data.serializedObject.ApplyModifiedProperties();
            Container.Clear();
        }

        public SerializableReorderableList BindProperty(SerializedProperty property)
        {
            Assert.IsTrue(Data.isArray);
            Data = property;
            return this;
        }

        public SerializableReorderableList SetTitle(string title)
        {
            Title.text = title;
            return this;
        }

        public SerializableReorderableList Build()
        {
            Assert.IsTrue(Data.isArray);
            for (int i = 0; i < Data.arraySize; i++)
            {
                SerializedProperty prop = Data.GetArrayElementAtIndex(i).FindPropertyRelative("Value");
                PropertyField propField = new PropertyField(prop)
                    .SetLabel();
                Container.Add(propField);
            }
            return this;
        }
    }
}
