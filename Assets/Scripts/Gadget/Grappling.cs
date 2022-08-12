using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gadgets
{
    public class Grappling : Gadget
    {
        [Header("Settings")]
        [SerializeField] [Range(0, 100f)] private float MaxDistance;
        [SerializeField] [Range(0, 10f)] private float MinDistance;
        [SerializeField] [Range(0, 100f)] private float PullSpeed;
        [SerializeField] [Range(0, 25f)] private float PullForce;
        [SerializeField] [Range(0, 25f)] private float PullDamper;
        [SerializeField] [Range(0, 100f)] private float RopeFlySpeed;

        [Header("Components")]
        [SerializeField] private Rigidbody BaseRig;
        [SerializeField] private Transform FirePoint;
        [SerializeField] private LineRenderer RopeLine;

        private Vector3 CurrantPoint { get; set; }
        private Vector3 GrapPoint;
        private Vector3 CurrantRopePosition;
        private bool ActionStarted;

        [SerializeField] private enum RopeStateType { Idle, Grapped, FlyForward, FlyBack }
        [SerializeField] private RopeStateType RopeState;
        private SpringJoint Joint;
        private float CurrantDistance;


        public override void ActionPoint(Vector3 point)
        {
            CurrantPoint = point;
        }
        public override void OnActionEnd(IEntity self)
        {
            ActionStarted = false;

            StopRope();
        }
        public override void OnActionStart(Action callback)
        {
            ActionStarted = true;

            StartRope(callback);
        }


        private Coroutine RopeFlyCoroutine;
        private void StartRope(Action callback)
        {
            if (RopeState == RopeStateType.Idle)
            {
                RopeFlyCoroutine = StartCoroutine(RopeFlyCour(callback));
            }
        }
        private void StopRope()
        {
            if(RopeState == RopeStateType.FlyForward)
            {
                StopCoroutine(RopeFlyCoroutine);
            }
            
            if(RopeState != RopeStateType.FlyBack)
            {
                RopeFlyCoroutine = StartCoroutine(RopeFlyBackCour());
            }
        }

        private IEnumerator RopeFlyCour(Action callback)
        {
            RopeState = RopeStateType.FlyForward;

            GrapPoint = CurrantPoint;
            CurrantDistance = Vector3.Distance(GrapPoint, FirePoint.position);
            bool canGrap = CurrantDistance < MaxDistance;
            if(!canGrap)
            {
                GrapPoint = FirePoint.position + (GrapPoint - FirePoint.position).normalized * MaxDistance;
                CurrantDistance = (FirePoint.position - GrapPoint).magnitude;
            }

            RopeLine.positionCount = 2;

            float flyTime = CurrantDistance / RopeFlySpeed;
            float time = 0;
            while(time < flyTime)
            {
                CurrantRopePosition = Vector3.Lerp(FirePoint.position, GrapPoint, time / flyTime);

                RopeLine.SetPosition(0, FirePoint.position);
                RopeLine.SetPosition(1, CurrantRopePosition);

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(0.1f);

            if (canGrap)
            {
                RopeState = RopeStateType.Grapped;
                CreatePullForce(callback);
                RopeFlyCoroutine = null;
            }
            else
            {
                RopeFlyCoroutine = StartCoroutine(RopeFlyBackCour());
            }
            yield break;
        }
        private IEnumerator RopeFlyBackCour()
        {
            RopeState = RopeStateType.FlyBack;

            CurrantDistance = Vector3.Distance(CurrantRopePosition, FirePoint.position);

            RopeLine.positionCount = 2;

            float flyTime = CurrantDistance / RopeFlySpeed;
            float time = 0;
            while (time < flyTime)
            {
                Vector3 currantRopePosition = Vector3.Lerp(CurrantRopePosition, FirePoint.position, time / flyTime);

                RopeLine.SetPosition(0, FirePoint.position);
                RopeLine.SetPosition(1, currantRopePosition);

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            CurrantRopePosition = FirePoint.position;

            DestroyPullForce();
            RopeState = RopeStateType.Idle;

            RopeFlyCoroutine = null;
            yield break;
        }

        private void CreatePullForce(Action callback)
        {
            GrapPoint = CurrantPoint;

            Joint = BaseRig.gameObject.AddComponent<SpringJoint>();
            Joint.autoConfigureConnectedAnchor = false;
            Joint.connectedAnchor = CurrantPoint;

            CurrantDistance = Vector3.Distance(GrapPoint, FirePoint.position);

            Joint.maxDistance = CurrantDistance;
            Joint.minDistance = MinDistance;

            Joint.spring = PullForce;
            Joint.damper = PullDamper;
            Joint.massScale = 1f;

            StartCoroutine(PullCour());

            callback?.Invoke();
        }
        private void DestroyPullForce()
        {
            RopeLine.positionCount = 0;
            Destroy(Joint);
        }
        private IEnumerator PullCour()
        {
            while(ActionStarted)
            {
                if (CurrantDistance > MinDistance)
                {
                    CurrantDistance -= PullSpeed * Time.fixedDeltaTime;
                    Joint.maxDistance = CurrantDistance;
                }
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private void LateUpdate()
        {
            if (ActionStarted && RopeState == RopeStateType.Grapped)
            {
                RopeLine.positionCount = 2;
                RopeLine.SetPosition(0, FirePoint.position);
                RopeLine.SetPosition(1, GrapPoint);
            }
        }

        public override void Init()
        {
            
        }


    }
}
