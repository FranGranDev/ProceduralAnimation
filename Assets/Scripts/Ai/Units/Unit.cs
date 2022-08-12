using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    delegate void UnitAction();

    public abstract class Unit : MonoBehaviour
    {
        //----State Machine---
        public enum States { Idle, Move, Attack }
        public States State
        {
            get
            {
                return state;
            }
            set
            {
                if (stateExecute != null)
                {
                    switch (state)
                    {
                        case States.Attack:
                            stateExecute -= Attacking;
                            break;
                        case States.Idle:
                            stateExecute -= Staying;
                            break;
                        case States.Move:
                            stateExecute -= Moving;
                            break;
                    }
                }
                state = value;
                switch (state)
                {
                    case States.Attack:
                        stateExecute += Attacking;
                        break;
                    case States.Idle:
                        stateExecute += Staying;
                        break;
                    case States.Move:
                        stateExecute += Moving;
                        break;
                }
            }
        }
        [SerializeField] protected States state;

        public void SetState(States state)
        {
            State = state;
        }
        public virtual void OnStateEnd(States state)
        {

        }
        public virtual void OnStateStart(States state)
        {

        }


        private UnitAction stateExecute;
        //----------------------

        public abstract void Moving();
        public abstract void Staying();
        public abstract void Attacking();

        private void FixedUpdate()
        {
            stateExecute?.Invoke();
        }
    }
}
