using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public interface IUnitBehavior
    {
        void Attack(IEntity entity);
        void Moving(Vector3 point);
        void Stay();
    }
}
