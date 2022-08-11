using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class KeybordInput : BaseInput
    {
        protected override void DoInput()
        {
            if (MovementTarget != null)
            {
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");

                MovementTarget.Move(new Vector3(0, 0, y), x);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    MovementTarget.Jump();
                }
            }
            if (GadgetTarget != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    GadgetTarget.ActionPoint(hit.point);

                    if (Input.GetMouseButtonDown(0))
                    {
                        GadgetTarget.OnActionStart(null);
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        GadgetTarget.OnActionEnd();
                    }
                }
            }
        }

    }
}