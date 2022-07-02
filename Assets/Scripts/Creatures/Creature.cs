using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public abstract class Creature : MonoBehaviour, IMove
    {
        public abstract void Move(Vector3 direction, float rotation);
        public abstract void Jump();
        public abstract void ActionPoint(Vector3 point);
        public abstract void OnActionStart();
        public abstract void OnActionEnd();
        public abstract void ChangeGadget(bool right);

        public void DoAction()
        {
           
        }
        public MoveState State { get; set; }
        public abstract void StateSelect();
        public abstract void StateExecute();

        public abstract void Init();

        private void Start()
        {
            Init();
        }
        private void FixedUpdate()
        {
            StateSelect();
            StateExecute();
        }
        private void Update()
        {

        }


    }
}