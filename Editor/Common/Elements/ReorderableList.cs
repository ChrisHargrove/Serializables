using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    public class ReorderableList : VisualElement
    {
        public ReorderableList()
        {
            StyleSheet serializableStyles = AssetDatabaseExtensions.LoadFirstAsset<StyleSheet>("SerializableStyles");
            this.AddUssSheet(serializableStyles)
                .AddUssClass("reorderable-list")
                .AddChild(new VisualElement()
                    .SetName("Header")
                    .AddUssSheet(serializableStyles)
                    .AddUssClass("header")
                    .AddChild(new Label()
                        .SetName("Title")
                        .AssignTo(ref title)))
                .AddChild(new VisualElement()
                    .SetName("Content")
                    .AddUssSheet(serializableStyles)
                    .AddUssClass("content")
                    .AssignTo(ref content))
                .AddChild(new VisualElement()
                    .SetName("ControlBar")
                    .AddUssSheet(serializableStyles)
                    .AddUssClass("control-bar")
                    .AddChild(new VisualElement()
                        .SetName("ButtonHolder")
                        .AddUssSheet(serializableStyles)
                        .AddUssClass("button-holder")
                        .AddChild(new Button(ClearElements)
                            .SetName("Clear")
                            .AddUssSheet(serializableStyles)
                            .AddUssClass("clear-icon")
                            .AddUssClass("square-button")
                            .SetTooltip("Remove all elements...")
                            .AssignTo(ref clearButton))
                        .AddChild(new Button(RemoveElement)
                            .SetName("Remove")
                            .AddUssSheet(serializableStyles)
                            .AddUssClass("remove-icon")
                            .AddUssClass("square-button")
                            .SetTooltip("Remove selected element...")
                            .AssignTo(ref removeButton))
                        .AddChild(new Button(AddElement)
                            .SetName("Add")
                            .AddUssSheet(serializableStyles)
                            .AddUssClass("add-icon")
                            .AddUssClass("square-button")
                            .SetTooltip("Add new element...")
                            .AssignTo(ref addButton))));

            Undo.undoRedoEvent += UndoRedoPerformed;
        }

        public ReorderableList(SerializedProperty property) : this()
        {
            Data = property;
        }

        private const string UndoName = "AddRemoveReorderableElement";

        private readonly Label title;
        private readonly VisualElement content;
        private readonly Button addButton;
        private readonly Button removeButton;
        private readonly Button clearButton;

        private SerializedProperty Data { get; set; }
        private bool IsReorderable { get; set; }
        private VisualElement CurrentlySelectedElement { get; set; }

        private Func<VisualElement> OnAddElement { get; set; }
        private Func<SerializedProperty, VisualElement> OnCreateElement { get; set; }
        private Action<VisualElement, IEnumerable<VisualElement>> OnClearElements { get; set; }

        private void AddElement()
        {
            Assert.IsNotNull(OnAddElement);
            Undo.RegisterCompleteObjectUndo(Data.serializedObject.targetObject, UndoName);
            content.Add(TryMakeReorderable(OnAddElement()));
        }

        private void RemoveElement()
        {
            if (CurrentlySelectedElement == null)
            {
                return;
            }
            Undo.RegisterCompleteObjectUndo(Data.serializedObject.targetObject, UndoName);
            int elementIndex = content.IndexOf(CurrentlySelectedElement);

            //NOTE: This might not be the best way to handle this
            //might want to come back and see if there is a more efficient way
            //to handle this...
            List<PropertyField> propertyFields = content.Query<PropertyField>().ToList();
            for (int i = 0; i < propertyFields.Count; i++)
            {
                propertyFields[i].Unbind();
            }

            Data.DeleteArrayElementAtIndex(elementIndex);
            content.Remove(CurrentlySelectedElement);
            CurrentlySelectedElement = null;

            Data.serializedObject.ApplyModifiedProperties();

            propertyFields = content.Query<PropertyField>().ToList();
            for (int i = 0; i < propertyFields.Count - 1; i++)
            {
                propertyFields[i].BindProp(Data.GetArrayElementAtIndex(i).FindPropertyRelative("Value"));
            }
        }

        private void ClearElements()
        {
            Undo.RegisterCompleteObjectUndo(Data.serializedObject.targetObject, UndoName);
            OnClearElements?.Invoke(content, content.Children());
        }

        private void SelectElement(ReorderableWrapper wrapper)
        {
            CurrentlySelectedElement?.RemoveUssClass("selected");
            CurrentlySelectedElement = wrapper.AddUssClass("selected");
        }

        private void ReorderElements(int oldIndex, int newIndex)
        {
            //NOTE: This might not be the best way to handle this
            //might want to come back and see if there is a more efficient way
            //to handle this...
            List<PropertyField> propertyFields = content.Query<PropertyField>().Where(elem => elem.parent == content || elem.parent is ReorderableWrapper).ToList();
            for (int i = 0; i < propertyFields.Count; i++)
            {
                propertyFields[i].Unbind();
            }

            Data.MoveArrayElement(oldIndex, newIndex);
            Data.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < propertyFields.Count; i++)
            {
                propertyFields[i].BindProp(Data.GetArrayElementAtIndex(i).FindPropertyRelative("Value"));
            }
        }

        public ReorderableList BindProperty(SerializedProperty property)
        {
            Assert.IsTrue(Data.isArray);
            Data = property;
            return this;
        }

        public ReorderableList SetReorderable(bool isReorderable = true)
        {
            IsReorderable = isReorderable;
            return this;
        }

        public ReorderableList SetTitle(string text)
        {
            title.text = text;
            return this;
        }

        public ReorderableList SetOnAddElement(Func<VisualElement> onAddElement)
        {
            OnAddElement = onAddElement;
            return this;
        }

        public ReorderableList SetOnCreateElement(Func<SerializedProperty, VisualElement> onCreateElement)
        {
            OnCreateElement = onCreateElement;
            return this;
        }

        public ReorderableList EnableAddRemoveElements(bool enabled = true)
        {
            addButton.Display(enabled);
            removeButton.Display(enabled);
            return this;
        }

        public ReorderableList SetOnClearElements(Action<VisualElement, IEnumerable<VisualElement>> onClearElements)
        {
            OnClearElements = onClearElements;
            return this;
        }

        public ReorderableList Build()
        {
            Assert.IsTrue(Data.isArray);
            Assert.IsNotNull(OnCreateElement);
            for (int i = 0; i < Data.arraySize; i++)
            {
                VisualElement propField = OnCreateElement(Data.GetArrayElementAtIndex(i));
                content.Add(TryMakeReorderable(propField));
            }
            return this;
        }

        private VisualElement TryMakeReorderable(VisualElement propertyField)
        {
            //TODO: Add the drag bars image in here instead of a coloured background.
            if (IsReorderable)
            {
                propertyField.FlexGrow(true)
                    .SetMargin(0, 4, 0, 0);

                return new ReorderableWrapper(propertyField)
                    .SetOnReordered(ReorderElements)
                    .SetOnSelected(SelectElement);
            }
            return propertyField;
        }

        private void UndoRedoPerformed(in UndoRedoInfo undo)
        {
            if (undo.undoName == UndoName)
            {
                try
                {
                    Data.serializedObject.Update();
                }
                catch (Exception ex)
                {

                }

                content.Clear();
                Build();
                this.Bind(Data.serializedObject);
            }
        }
    }
}
