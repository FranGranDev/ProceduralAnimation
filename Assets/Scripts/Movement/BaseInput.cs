using System;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public abstract class BaseInput : MonoBehaviour
    {
        [SerializeField] protected GameObject _target;
        public IMovement Target { get; private set; }

        protected abstract void DoInput();

        protected void GetTarget()
        {
            try
            {
                Target = _target.GetComponent<IMovement>();
            }
            catch
            {
                Debug.LogError("Error cast to target!");
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