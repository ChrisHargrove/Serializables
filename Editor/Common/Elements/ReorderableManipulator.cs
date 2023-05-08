using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    internal class ReorderableManipulator : PointerManipulator
    {
        public ReorderableManipulator(ReorderableWrapper reorderableWrapper)
        {
            ReorderableWrapper = reorderableWrapper;
            this.target = ReorderableWrapper.Handle;

            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse });
        }

        private ReorderableWrapper ReorderableWrapper { get; }
        private Vector2 TargetStartPosition { get; set; }
        private Vector3 PointerStartPosition { get; set; }
        private bool Enabled { get; set; }
        private float ContainerContentYOffset => ReorderableWrapper.parent.contentRect.yMin;
        private float FieldStartYOffset => ReorderableWrapper.layout.yMin - ContainerContentYOffset;

        private Action<int, int> OnReorderProperty { get; set; }
        private Action<ReorderableWrapper> OnElementSelected { get; set; }

        public ReorderableManipulator SetOnReorderProperty(Action<int, int> onReorderProperty)
        {
            OnReorderProperty = onReorderProperty;
            return this;
        }

        public ReorderableManipulator SetOnElementSelected(Action<ReorderableWrapper> onElementSelected)
        {
            OnElementSelected = onElementSelected;
            return this;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        private void PointerDownHandler(PointerDownEvent evt)
        {
            TargetStartPosition = ReorderableWrapper.transform.position;
            PointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            OnElementSelected?.Invoke(ReorderableWrapper);
            Enabled = true;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (Enabled && target.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - PointerStartPosition;

                ReorderableWrapper.transform.position = new Vector2(
                    0,
                    Mathf.Clamp(TargetStartPosition.y + pointerDelta.y, -FieldStartYOffset, ReorderableWrapper.parent.contentRect.height - ReorderableWrapper.contentRect.height - ContainerContentYOffset - FieldStartYOffset));

                VisualElement closestField = FindClosestField(ReorderableWrapper.parent.Children().Where(OverlapsTarget));
                if (closestField != null)
                {
                    int closestFieldIndex = ReorderableWrapper.parent.IndexOf(closestField);
                    int propertyFieldIndex = ReorderableWrapper.parent.IndexOf(ReorderableWrapper);
                    if (closestFieldIndex < propertyFieldIndex)
                    {
                        closestField.PlaceInFront(ReorderableWrapper);
                        OnReorderProperty?.Invoke(closestFieldIndex, propertyFieldIndex);
                        PointerStartPosition -= new Vector3(0, ReorderableWrapper.layout.height);
                    }
                    else
                    {
                        closestField.PlaceBehind(ReorderableWrapper);
                        OnReorderProperty?.Invoke(propertyFieldIndex, closestFieldIndex);
                        PointerStartPosition += new Vector3(0, ReorderableWrapper.layout.height);
                    }
                    pointerDelta = evt.position - PointerStartPosition;
                    TargetStartPosition = Vector2.zero;

                    ReorderableWrapper.transform.position = new Vector2(
                        0,
                        Mathf.Clamp(TargetStartPosition.y + pointerDelta.y, -FieldStartYOffset, ReorderableWrapper.parent.contentRect.height - ReorderableWrapper.contentRect.height - ContainerContentYOffset - FieldStartYOffset));
                }
            }
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (Enabled && target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
            }
        }

        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (Enabled)
            {
                ReorderableWrapper.transform.position = Vector2.zero;
                Enabled = false;
            }
        }

        private bool OverlapsTarget(VisualElement field)
        {
            return field != ReorderableWrapper && ReorderableWrapper.worldBound.Overlaps(field.worldBound);
        }

        private VisualElement FindClosestField(IEnumerable<VisualElement> fields)
        {
            float bestDistanceSq = float.MaxValue;
            VisualElement closest = null;
            foreach (VisualElement field in fields)
            {
                float distanceSq = field.layout.yMin - (ReorderableWrapper.layout.yMin + ReorderableWrapper.transform.position.y);
                if (distanceSq < bestDistanceSq && Mathf.Abs(distanceSq) < 4)
                {
                    bestDistanceSq = distanceSq;
                    closest = field;
                }
            }
            return closest;
        }
    }
}
