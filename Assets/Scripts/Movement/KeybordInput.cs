using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class KeybordInput : BaseInput
    {
        protected override void DoInput()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            
            Target.Move(new Vector3(0, 0, y), x);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                Target.Jump();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Target.ActionPoint(hit.point);

                if (Input.GetMouseButtonDown(0))
                {
                    Target.OnActionStart();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Target.OnActionEnd();
                }
            }
        }

    }
}