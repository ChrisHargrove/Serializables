using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal class ReorderableWrapper : VisualElement
    {
        public ReorderableWrapper()
        {
            StyleSheet serializableStyles = AssetDatabaseExtensions.LoadFirstAsset<StyleSheet>("SerializableStyles");
            this.AddUssSheet(serializableStyles)
                .AddUssClass("wrapper")
                .SetName("ReorderableWrapper")
                .AlignItems(Align.Center)
                .AddChild(new VisualElement()
                    .AddUssSheet(serializableStyles)
                    .AddUssClass("handle")
                    .SetName("ReorderableHandle")
                    .AssignTo(ref handle));

            new ReorderableManipulator(this)
                .SetOnReorderProperty(Reorder)
                .SetOnElementSelected(Select);
        }

        public ReorderableWrapper(VisualElement propertyField) : this()
        {
            this.Add(propertyField);
        }

        private readonly VisualElement handle;
        public VisualElement Handle => handle;

        private Action<int, int> OnReordered { get; set; }
        private Action<ReorderableWrapper> OnSelected { get; set; }

        public ReorderableWrapper SetOnReordered(Action<int, int> onReorder)
        {
            OnReordered = onReorder;
            return this;
        }

        public ReorderableWrapper SetOnSelected(Action<ReorderableWrapper> onSelect)
        {
            OnSelected = onSelect;
            return this;
        }

        private void Reorder(int oldIndex, int newIndex)
        {
            OnReordered?.Invoke(oldIndex, newIndex);
        }

        private void Select(ReorderableWrapper wrapper)
        {
            OnSelected?.Invoke(wrapper);
        }
    }
}
