using UnityEngine;

namespace Assets.Scripts.Movement
{
    public interface ITouchMovement
    {
        public enum MouseButtons { Left, Right }

        public delegate void TouchAction(Vector2 screenPos, MouseButtons button);
        public delegate void DragAction(Vector2 firstTap, Vector2 secondTap, MouseButtons button);

        TouchAction OnClick { get; set; }
        TouchAction OnStartSelect { get; set; }

        DragAction OnEndSelect { get; set; }
        DragAction OnDragBox { get; set; }
    }
}
