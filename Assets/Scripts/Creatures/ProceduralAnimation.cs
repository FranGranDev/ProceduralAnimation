using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class ProceduralAnimation : MonoBehaviour
    {
        [Header("Move Animation")]
        public float StepHeight;
        [Range(0.001f, 0.25f)]
        public float LegMoveTime;
        public AnimationCurve LegsMoveHorizontal;
        public AnimationCurve LegsMoveVertical;
        [Header("Legs Settings")]
        [Range(0f, 10f)] public float LegMaxDistance;
        [Range(0.75f, 1.25f)] public float StepRatio;
        private float MaxDistance()
        {
            return LegMaxDistance;
        }
        private float MoveTime()
        {
            return LegMoveTime;
        }
        private Vector3 StepPosition(Point leg)
        {
            return leg.centerPosition + Rig.velocity * (LegMaxDistance * LegMoveTime * StepRatio);
        }

        public Vector3 LegsNormal { get; private set; }
        public Vector3 LegsCenter { get; private set; }
        public int LegsOnGround { get; private set; }
        public float RotationSpeed { get; private set; }

        private Vector3 prevRotation;

        private bool CanMoveLeg(Point leg)
        {
            return !MovingLegs.Contains(leg) && MovingLegs.Count <= 3;
        }


        [Header("Components")]
        [SerializeField] private Transform Base;
        [SerializeField] private Point[] _legs;
        private List<Point> MovingLegs = new List<Point>();
        
        public Point[] Legs => _legs;
        [SerializeField] private Rigidbody Rig;

        public void Simulate(Vector3 direction)
        {
            CalculateSpeed();
            CalculateLegsOnGround();
            CalculateNormal();

            for (int i = 0; i < _legs.Length; i++)
            {
                _legs[i].UpdateTargetPoint();
                _legs[i].UpdateCenterPoint(direction);

                Point leg = _legs[i];
                Vector3 dir = (leg.targetPosition - leg.centerPosition);
                if (dir.magnitude > MaxDistance() && CanMoveLeg(leg))
                {
                    PointMoveData moveData = new PointMoveData(LegsMoveHorizontal, LegsMoveVertical, StepHeight, LegMoveTime);
                    leg.MoveTargetPoint(StepPosition(leg), moveData);
                    WaitForMoveNextLeg(leg);
                }
            }
        }

        private void WaitForMoveNextLeg(Point leg)
        {
            StartCoroutine(WaitForMoveNextLegCour(leg));
        }
        private IEnumerator WaitForMoveNextLegCour(Point leg)
        {
            if (!MovingLegs.Contains(leg))
                MovingLegs.Add(leg);
            yield return new WaitForSeconds(MoveTime());
            MovingLegs.Remove(leg);
            yield break;
        }

        private void CalculateNormal()
        {
            Vector3 Normal = Vector3.zero;

            List<Vector3> LegsOnGroundPos = new List<Vector3>();
            for (int i = 0; i < Legs.Length; i++)
            {
                if (Legs[i].OnGround)
                {
                    LegsOnGroundPos.Add(Legs[i].targetPosition * 0.5f + Legs[i].centerPosition * 0.5f);
                }
            }

            if (LegsOnGroundPos.Count < Legs.Length / 2)
            {
                LegsNormal = Vector3.Lerp(LegsNormal, Vector3.up, 0.1f);
                return;
            }

            Vector3[] vectors = new Vector3[LegsOnGroundPos.Count];
            for (int i = 0; i < LegsOnGroundPos.Count - 1; i++)
            {
                vectors[i] = (LegsOnGroundPos[i] - LegsOnGroundPos[i + 1]).normalized;
            }
            for (int i = 0; i < vectors.Length - 1; i++)
            {
                Normal += Vector3.Cross(vectors[i], vectors[i + 1]).normalized;
            }
            Normal.Normalize();

            LegsNormal = Vector3.Lerp(LegsNormal, -Normal, 0.25f);
        }
        private void CalculateSpeed()
        {
            RotationSpeed = (Base.rotation.eulerAngles - prevRotation).magnitude;
            prevRotation = Base.rotation.eulerAngles;
        }
        private void CalculateLegsOnGround()
        {
            LegsOnGround = 0;
            foreach (Point leg in _legs)
            {
                if (leg.OnGround)
                {
                    LegsOnGround++;
                }
            }
        }



        private void Start()
        {
            LegsNormal = Vector3.up;
        }
    }
}
