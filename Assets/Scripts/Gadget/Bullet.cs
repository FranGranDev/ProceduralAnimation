using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gadgets
{
    public class Bullet : MonoBehaviour
    {

        [Header("Components")]
        [SerializeField] private Rigidbody Rig;

        private bool Fired;
        private BulletData Data;

        public void Fire(BulletData data)
        {
            Data = data;
            Fired = true;
            StartCoroutine(FlyCour());
        }
        private float CalculateDistance()
        {

            float distance = 0;
            int stepCount = 25;
            Vector3 prevPos = Data.Start;
            for (int i = 0; i < stepCount; i++)
            {
                float ratio = (float)i / (float)stepCount;
                Vector3 point = Vector3.Lerp(Data.Start, Data.Target, ratio);
                point += Data.Direction * Data.FlyCurve.Evaluate(ratio) * Data.Height;

                distance += (prevPos - point).magnitude;
                prevPos = point;
            }

            return distance;
        }
        private IEnumerator FlyCour()
        {
            float distance = CalculateDistance();
            float flyTime = distance / Data.Speed;
            float time = 0;
            Vector3 prevPos = Data.Start;
            while (time < flyTime)
            {
                time += Time.fixedDeltaTime;

                float ratio = time / flyTime;
                Vector3 point = Vector3.Lerp(Data.Start, Data.Target, ratio);
                point += Data.Direction * Data.FlyCurve.Evaluate(ratio) * Data.Height;
                Vector3 forward = (prevPos - point).normalized;
                prevPos = point;

                transform.position = point;
                transform.forward = forward;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForFixedUpdate();
            Destroy(gameObject);
            yield break;
        }


    }
    public struct BulletData
    {
        public GameObject Own;
        public AnimationCurve FlyCurve;
        public float Height;
        public Vector3 Target;
        public Vector3 Start;
        public Vector3 Direction;

        public float Speed;

        public BulletData(GameObject own, AnimationCurve flyCurve, float height, Vector3 target, Vector3 start, Vector3 direction, float speed)
        {
            Own = own;
            FlyCurve = flyCurve;
            Height = height;
            Target = target;
            Start = start;
            Speed = speed;
            Direction = direction;
        }
    }
}
