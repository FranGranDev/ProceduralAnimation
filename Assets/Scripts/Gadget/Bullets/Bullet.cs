using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gadgets
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Bullet : MonoBehaviour
    {
        protected Rigidbody rig;

        protected BulletData data;

        protected float Distance
        {
            get
            {
                float distance = 0;
                int stepCount = 25;
                Vector3 prevPos = data.Start;
                for (int i = 0; i < stepCount; i++)
                {
                    float ratio = (float)i / (float)stepCount;
                    Vector3 point = Vector3.Lerp(data.Start, data.Target, ratio);
                    point += data.Direction * data.FlyCurve.Evaluate(ratio) * data.Height;

                    distance += (prevPos - point).magnitude;
                    prevPos = point;
                }

                return distance;
            }
        }

        public abstract void Fire(BulletData data);
        public abstract void OnHit(IEntity entity);
        public abstract void OnHit(GameObject obj);

        private void OnTriggerEnter(Collider other)
        {
            IEntity entity = null;
            if(other.TryGetComponent(out entity))
            {
                OnHit(entity);
            }
            else
            {
                OnHit(other.gameObject);
            }
        }

        private void Awake()
        {
            rig = GetComponent<Rigidbody>();
        }
    }
    public struct BulletData
    {
        public AnimationCurve FlyCurve;
        public float Height;
        public Vector3 Target;
        public Vector3 Start;
        public Vector3 Direction;

        public float Speed;

        public IEntity Self;

        public BulletData(AnimationCurve flyCurve, float height, 
        Vector3 target, Vector3 start, Vector3 direction, float speed, IEntity self)
        {
            FlyCurve = flyCurve;
            Height = height;
            Target = target;
            Start = start;
            Speed = speed;
            Direction = direction;
            Self = self;
        }
    }
}
