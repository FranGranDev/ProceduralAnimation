using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gadgets
{
    public class Rocket : Bullet
    {
        [SerializeField] protected float boomRadius;
        [SerializeField] protected float impulsePower;

        public override void Fire(BulletData data)
        {
            this.data = data;
            StartCoroutine(FlyCour());
        }
        public override void OnHit(IEntity entity)
        {
            if (entity.Compare(data.Self))
                return;
            Boom();
        }
        public override void OnHit(GameObject other)
        {
            //Boom();
        }

        private IEnumerator FlyCour()
        {
            float distance = Distance;
            float flyTime = distance / data.Speed;
            float time = 0;
            Vector3 prevPos = data.Start;
            while (time < flyTime)
            {
                time += Time.fixedDeltaTime;

                float ratio = time / flyTime;
                Vector3 point = Vector3.Lerp(data.Start, data.Target, ratio);
                point += data.Direction * data.FlyCurve.Evaluate(ratio) * data.Height;
                Vector3 forward = (prevPos - point).normalized;
                prevPos = point;

                transform.position = point;
                transform.forward = forward;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForFixedUpdate();

            Boom();

            yield break;
        }
        public void Boom()
        {
            Collider[] others = Physics.OverlapSphere(transform.position, boomRadius);
            IEntity entity = null;
            foreach (Collider obj in others)
            {
                if(obj.TryGetComponent(out entity))
                {
                    Vector3 vector = (entity.Body.position - transform.position);
                    float distance = vector.magnitude;

                    Vector3 impulse = (vector.normalized + Vector3.up).normalized * impulsePower / (distance + 1);

                    if (!entity.Compare(data.Self))
                    {
                        entity.GetHit(new ImpulseHit(5, impulse));
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
