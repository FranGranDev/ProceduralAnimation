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
                if (actionsExecute != null)
                {
                    switch (state)
                    {
                        case States.Attack:
                            actionsExecute -= Attacking;
                            break;
                        case States.Idle:
                            actionsExecute -= Staying;
                            break;
                        case States.Move:
                            actionsExecute -= Moving;
                            break;
                    }
                }
                state = value;
                switch (state)
                {
                    case States.Attack:
                        actionsExecute += Attacking;
                        break;
                    case States.Idle:
                        actionsExecute += Staying;
                        break;
                    case States.Move:
                        actionsExecute += Moving;
                        break;
                }
            }
        }

        [SerializeField] protected States state;
       
        private UnitAction actionsExecute;

        //--------------------

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

        //----------------------

        public abstract void Moving();
        public abstract void Staying();
        public abstract void Attacking();

        private void FixedUpdate()
        {
            actionsExecute?.Invoke();
        }
    }
}
