using System;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public abstract class BaseInput : MonoBehaviour
    {
        [SerializeField] protected GameObject _target;
        public IMovement MovementTarget { get; private set; }
        public IGadget GadgetTarget { get; private set; }

        protected abstract void DoInput();

        protected void GetTarget()
        {
            try
            {
                MovementTarget = _target.GetComponent<IMovement>();
                GadgetTarget = _target.GetComponent<IGadget>();
            }
            catch
            {
                Debug.LogError("Error cast to movement target!");
            }
        }


        private void Awake()
        {
            GetTarget();
        }
        private void Update()
        {
            DoInput();
        }
    }
}