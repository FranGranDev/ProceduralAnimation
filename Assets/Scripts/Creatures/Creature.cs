using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public abstract class Creature : MonoBehaviour, IMovement, IEntity
    {
        //Entity
        public abstract int MaxHp { get; }
        public abstract int Hp { get; protected set; }
        public abstract bool Dead { get;}
        public abstract void Die();
        public abstract System.Action OnDie { get; set; }
        public abstract Transform Body { get; }

        //Movement
        public abstract void Move(Vector3 direction, float rotation);
        public abstract void Jump();

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