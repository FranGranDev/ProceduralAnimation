using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class TouchInput : MonoBehaviour, ITouchMovement
    {
        private const float MIN_DST_FOR_BOX_SELECT = 50;

        private enum States { Null, StartTap, BoxSelect}
        [SerializeField] private States state;

        public ITouchMovement.TouchAction OnClick { get; set; }
        public ITouchMovement.TouchAction OnStartSelect { get; set; }
        public ITouchMovement.DragAction OnEndSelect { get; set; }
        public ITouchMovement.DragAction OnDragBox { get; set; }


        private bool touched;
        private Vector3 startTapPosition;
        private ITouchMovement.MouseButtons startButton;

        private void MouseInput()
        {

            if (!touched && Input.GetMouseButtonDown(0))
            {
                OnStartTap(Input.mousePosition, ITouchMovement.MouseButtons.Left);
            }
            else if (!touched && Input.GetMouseButtonDown(1))
            {
                OnStartTap(Input.mousePosition, ITouchMovement.MouseButtons.Right);
            }

            if (touched && Input.GetMouseButtonUp((int)startButton))
            {
                OnEndTap(Input.mousePosition);
            }

            if (touched)
            {
                switch (state)
                {
                    case States.BoxSelect:
                        OnDragBox?.Invoke(startTapPosition, Input.mousePosition, startButton);
                        break;
                }
            }
        }

        private void OnStartTap(Vector2 position, ITouchMovement.MouseButtons button)
        {
            startTapPosition = position;
            state = States.StartTap;
            startButton = button;
            touched = true;

            StartCoroutine(WaitForBoxSelect());
        }
        private void OnEndTap(Vector2 position)
        {
            switch(state)
            {
                case States.BoxSelect:
                    OnEndSelect?.Invoke(startTapPosition, position, startButton);
                    break;
                case States.StartTap:
                    OnClick?.Invoke(position, startButton);
                    break;
            }

            state = States.Null;
            touched = false;
        }

        private IEnumerator WaitForBoxSelect()
        {
            while ((startTapPosition - Input.mousePosition).magnitude < MIN_DST_FOR_BOX_SELECT)
            {
                if(!touched)
                {
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }

            state = States.BoxSelect;
            OnStartSelect?.Invoke(Input.mousePosition, startButton);
            yield break;
        }

        private void Update()
        {
            MouseInput();
        }
    }
}
