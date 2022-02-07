using UnityEngine;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class ADraggable : PointerManipulator
    {
        public ADraggable(VisualElement target)
        {
            this.target = target;
        }

        Vector2 targetStartPosition;
        Vector3 pointerStartPosition;

        void PointerDownHandler(PointerDownEvent evt)
        {
            targetStartPosition = target.transform.position;
            pointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
        }

        void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (target.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;

                target.transform.position = new Vector2(
                    Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                    Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
            }
        }

        void PointerUpHandler(PointerUpEvent evt)
        {
            if (target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
            }
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        }
    }
}
