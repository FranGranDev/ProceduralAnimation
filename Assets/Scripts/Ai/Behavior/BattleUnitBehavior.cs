using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public class BattleUnitBehavior : IUnitBehavior
    {
        private IMovement movement;
        private IGadget gadget;
        private System.Action<Unit.States> onChangeState;
        private MonoBehaviour monoBehaviour;

        private bool firing;

        public BattleUnitBehavior(IMovement movement, IGadget gadget, System.Action<Unit.States> onChangeState, MonoBehaviour monoBehaviour)
        {
            this.movement = movement;
            this.gadget = gadget;
            this.onChangeState = onChangeState;
            this.monoBehaviour = monoBehaviour;
        }

        public void Attack(IEntity entity)
        {
            if(!firing)
            {
                if(entity.Equals(null))
                {
                    onChangeState(Unit.States.Idle);
                    return;
                }
                monoBehaviour.StartCoroutine(FireCour(entity.Body.position));
            }

            movement.Move(Vector3.zero, 0);
        }
        private IEnumerator FireCour(Vector3 point)
        {
            firing = true;

            gadget.OnActionStart(null);
            gadget.ActionPoint(point);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            gadget.OnActionEnd();

            yield return new WaitForSeconds(Random.Range(1f, 2f));
            firing = false;
            yield break;
        }


        public void Moving(Vector3 targetPoint)
        {
            if (Vector3.Distance(movement.Body.position, targetPoint) < 2)
            {
                onChangeState?.Invoke(Unit.States.Idle);
                return;
            }
            Vector3 direction = (targetPoint - movement.Body.position).normalized;
            float rotation = Mathf.Clamp(5 * Vector3.Dot(direction, movement.Body.right), -1, 1);
            direction = Vector3.forward * (1 - Mathf.Abs(rotation) * 0.25f);

            movement.Move(direction, rotation);
        }


        public void Stay()
        {
            movement.Move(Vector3.zero, 0);
        }
    }
}