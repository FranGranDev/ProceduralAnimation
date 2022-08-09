using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public interface ISelectable
    {
        Transform transform { get; }

        bool Selected { get; set; }
        void MoveTo(Vector3 position);
        void Add(List<ISelectable> units);
        void Clear();
    }
}
