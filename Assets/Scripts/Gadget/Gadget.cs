using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Gadgets
{
    public abstract class Gadget : MonoBehaviour, IGadget
    {
        public enum CreatureActionType { None, Jump }
        public CreatureActionType CallbackAction;

        public abstract void ActionPoint(Vector3 point);
        public abstract void OnActionStart(System.Action callback);
        public abstract void OnActionEnd();
        public void ChangeGadget(bool right)
        {

        }

        public abstract void Init();
        private void Start()
        {
            Init();
        }

    }
}