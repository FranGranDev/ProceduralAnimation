using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class Point : MonoBehaviour
    {
        [Header("Settings")]
        public float Lenght;
        [Header("Components")]
        [SerializeField] private Transform centerPoint;
        [SerializeField] private Transform targetPoint;
        public Vector3 targetPosition { get; private set; }
        public Vector3 centerPosition { get; private set; }
        public Vector3 prevCenterPosition { get; private set; }

        public float CurrantLenght()
        {
            return (Bones[0].position - Bones[Bones.Length - 1].position).magnitude;
        }

        public Vector3 RayVectorDown()
        {
            return -(transform.up).normalized;
        }
        private Vector3 CurrantVector;

        private Vector3 GroundNormal;

        public Transform[] Bones;
        public float LastMovedTime;
        public bool OnGround;
        public bool OnJumpAnimation;

        private Coroutine MovePointCoroutine;

        public void MoveTargetPoint(Vector3 position, PointMoveData moveData)
        {
            if (MovePointCoroutine == null && !OnJumpAnimation)
            {
                MovePointCoroutine = StartCoroutine(MovePointCour(position, moveData));

                LastMovedTime = Time.time;
            }
        }
        public void SetTargetPointPosition(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, RayVectorDown(), out hit, Lenght, 1 << 3))
            {
                this.targetPosition = hit.point;
            }
            LastMovedTime = Time.time;
        }

        private IEnumerator MovePointCour(Vector3 position, PointMoveData moveData)
        {
            RaycastHit hit;
            if (Physics.Raycast(position - RayVectorDown() * Lenght, RayVectorDown(), out hit, Lenght * 2, 1 << 3))
            {
                Vector3 startPosition = targetPoint.position;
                Vector3 endPosition = hit.point;



                float time = 0f;
                float moveTime = moveData.MoveTime;

                this.targetPosition = startPosition;
                while (time < moveTime)
                {
                    Vector3 Height = transform.up * moveData.Vertical.Evaluate(time / moveTime) * moveData.StepHeight;

                    float execute = moveData.Horizontal.Evaluate(time / moveTime);
                    this.targetPosition = startPosition * (1 - execute) + endPosition * execute + Height;

                    time += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                this.targetPosition = endPosition;


            }
            MovePointCoroutine = null;
            yield break;
        }

        public void UpdateCenterPoint(Vector3 forward)
        {
            CurrantVector = RayVectorDown();
            if (centerPosition != prevCenterPosition)
            {
                prevCenterPosition = centerPoint.position;
            }
            RaycastHit hit;
            if (Physics.Raycast(centerPoint.position, forward, out hit, Lenght * (OnGround ? 0.5f : 0.125f), 1 << 3))
            {
                centerPosition = hit.point;
                GroundNormal = hit.normal;

                if (!OnGround)
                {
                    OnLandGround();
                }

            }
            else if (Physics.Raycast(centerPoint.position - CurrantVector, CurrantVector, out hit, Lenght * (OnGround ? 2 : 1.25f), 1 << 3))
            {
                centerPosition = hit.point;
                GroundNormal = hit.normal;

                if (!OnGround)
                {
                    OnLandGround();
                }

            }
            else if (OnGround)
            {
                OnLeaveGround();
            }
        }
        public void UpdateTargetPoint()
        {
            if (!OnGround && !OnJumpAnimation)
            {
                targetPosition = centerPoint.position;
            }
            targetPoint.position = targetPosition;
        }

        private void OnLeaveGround()
        {
            if (MovePointCoroutine != null)
            {
                StopCoroutine(MovePointCoroutine);
            }
            if (!OnJumpAnimation)
            {
                StartCoroutine(OnLeaveGroundCour());
            }
        }
        private IEnumerator OnLeaveGroundCour()
        {
            OnJumpAnimation = true;
            OnGround = false;

            float time = 0f;
            float moveTime = 0.5f;

            while (time < moveTime)
            {
                float execute = time / moveTime;
                targetPosition = Vector3.Lerp(targetPoint.position, centerPoint.position, execute);

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            targetPosition = centerPoint.position;


            OnJumpAnimation = false;
            MovePointCoroutine = null;
            yield break;
        }
        private void OnLandGround()
        {
            if (MovePointCoroutine != null)
            {
                StopCoroutine(MovePointCoroutine);
            }
            if (!OnJumpAnimation)
            {
                StartCoroutine(OnLandGroundCour());
            }
        }
        private IEnumerator OnLandGroundCour()
        {
            OnGround = true;

            RaycastHit hit;
            if (Physics.Raycast(centerPoint.position, RayVectorDown(), out hit, Lenght, 1 << 3))
            {
                OnJumpAnimation = true;

                Vector3 endPosition = hit.point;

                float time = 0f;
                float moveTime = 0.5f;

                targetPosition = targetPoint.position;
                while (time < moveTime)
                {
                    float execute = time / moveTime;
                    targetPosition = Vector3.Lerp(targetPoint.position, endPosition, execute);

                    time += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                targetPosition = endPosition;
            }
            OnJumpAnimation = false;
            MovePointCoroutine = null;
            yield break;
        }

        void Start()
        {
            SetTargetPointPosition(targetPoint.position);
            prevCenterPosition = centerPosition;
        }
    }

    public struct PointMoveData
    {
        public readonly AnimationCurve Horizontal;
        public readonly AnimationCurve Vertical;
        public readonly float StepHeight;
        public readonly float MoveTime;

        public PointMoveData(AnimationCurve horizontal, AnimationCurve vertical, float stepHeight, float moveTime)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            StepHeight = stepHeight;
            MoveTime = moveTime;
        }
    }
}