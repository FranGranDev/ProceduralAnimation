using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class MouseInput : BaseInput
    {
        protected override void DoInput()
        {
            Vector3 TargetPoint = transform.position;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                TargetPoint = hit.point;
            }
        }

    }
}