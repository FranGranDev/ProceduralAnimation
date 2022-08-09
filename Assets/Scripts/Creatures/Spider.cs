using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Gadgets;

namespace Assets.Scripts.Creatures
{
    public class Spider : Creature
    {
        [Range(0, 15f)] [SerializeField] private float Speed;
        [Range(0, 360f)] [SerializeField] private float RotationSpeed;
        [Range(0, 270f)] [SerializeField] private float AirRotationSpeed;
        [Range(0, 100f)] [SerializeField] private float JumpVelocity;

        [Range(0, 3f)] [SerializeField] private float MinHeight;
        [Range(0, 1f)] [SerializeField] private float BodyUpSpeed;

        public MoveState ShowState;

        public float CurrantBodyUpSpeed { get; private set; }
        public Vector3 Direction { get; private set; }
        public float CurrantSpeed { get; private set; }
        public override Transform Body => Base;

        [Header("Components")]
        [SerializeField] private Gadget Weapon;
        [SerializeField] private Transform Base;
        [SerializeField] private Rigidbody Rig;
        [SerializeField] private ProceduralAnimation LegsAnim;

        public override void Move(Vector3 direction, float rotation)
        {
            direction.y = 0f;

            switch(State)
            {
                case MoveState.OnGround:
                    Base.Rotate(Vector3.up, rotation * RotationSpeed * Time.fixedDeltaTime);
                    Rig.velocity = Vector3.Lerp(Rig.velocity, Base.TransformDirection(direction) * Speed, 0.25f);
                    CurrantSpeed = Rig.velocity.magnitude / Speed;
                    break;
                case MoveState.Fly:
                    Base.Rotate(Vector3.up, rotation * AirRotationSpeed * Time.fixedDeltaTime);
                    transform.Rotate(Base.right, direction.z * AirRotationSpeed * Time.fixedDeltaTime);
                    CurrantSpeed = Rig.velocity.magnitude / Speed;
                    break;
                case MoveState.Jumping:
                    Base.Rotate(Vector3.up, rotation * AirRotationSpeed * Time.fixedDeltaTime);
                    transform.Rotate(Base.right, direction.z * AirRotationSpeed * Time.fixedDeltaTime);
                    CurrantSpeed = Rig.velocity.magnitude / Speed;
                    break;
            }
        } //Use direction in localSpace!

        public override void Jump()
        {
            if (State != MoveState.OnGround)
                return;
            State = MoveState.Fly;
            Rig.AddForce(transform.up * JumpVelocity, ForceMode.VelocityChange);
            SetState(MoveState.Jumping);
            StartCoroutine(JumpDelayCour());
        }
        private IEnumerator JumpDelayCour()
        {
            yield return new WaitForSeconds(0.5f);
            while(LegsAnim.LegsOnGround < LegsAnim.Legs.Length / 2)
            {
                if (State == MoveState.Fly)
                    yield break;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(0.5f);
            SetState(MoveState.OnGround);
            yield break;
        }
        public override void ActionPoint(Vector3 point)
        {
            if (Weapon == null)
                return;
            Weapon.ActionPoint(point);
        }
        public override void OnActionStart()
        {
            if (Weapon == null)
                return;
            System.Action callback = null;
            switch(Weapon.CallbackAction)
            {
                case Gadget.CreatureActionType.Jump:
                    callback = Jump;
                    break;
            }
            Weapon.OnActionStart(callback);
        }
        public override void OnActionEnd()
        {
            if (Weapon == null)
                return;
            Weapon.OnActionEnd();
        }
        public override void ChangeGadget(bool right)
        {
            
        }

        public override void StateSelect()
        {
            ShowState = State;

            switch (State)
            {
                case MoveState.Fly:
                    if (LegsAnim.LegsOnGround >= LegsAnim.Legs.Length - 1)
                    {
                        SetState(MoveState.OnGround);
                    }
                    break;
                case MoveState.OnGround:
                    if (LegsAnim.LegsOnGround < LegsAnim.Legs.Length - 1)
                    {
                        SetState(MoveState.Fly);
                    }
                    break;
                case MoveState.Jumping:
                    {
                        if (LegsAnim.LegsOnGround == 0)
                        {
                            SetState(MoveState.Fly);
                        }
                    }
                    break;
            }
        }
        public override void StateExecute()
        {
            LegsAnim.Simulate(Direction);

            switch (State)
            {
                case MoveState.OnGround:
                    SetBodyPosition();
                    SetBodyNormalRotation();
                    break;
                case MoveState.Fly:
                    SetZeroBodyUpSpeed();
                    SetBodyFlyRotation();
                    break;
            }
        }

        public override void Init()
        {

        }

        private void SetState(MoveState state)
        {
            if (State == state)
                return;
            State = state;
            switch (State)
            {
                case MoveState.Fly:
                    Rig.useGravity = true;
                    break;
                case MoveState.OnGround:
                    Rig.useGravity = false;
                    break;
                case MoveState.Jumping:
                    Rig.useGravity = true;
                    break;
            }
        }
        private void SetBodyPosition()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 5, 1 << 3))
            {
                if (hit.distance > MinHeight * 1.1f)
                {
                    Rig.AddForce(-transform.up * CurrantBodyUpSpeed, ForceMode.VelocityChange);
                }
                else if (hit.distance < MinHeight * 0.9f)
                {
                    Rig.AddForce(transform.up * CurrantBodyUpSpeed, ForceMode.VelocityChange);
                }
            }
            CurrantBodyUpSpeed = Mathf.Lerp(CurrantBodyUpSpeed, BodyUpSpeed, 0.05f);
        }
        private void SetZeroBodyUpSpeed()
        {
            CurrantBodyUpSpeed = Mathf.Lerp(CurrantBodyUpSpeed, 0, 0.025f);
        }
        private void SetBodyNormalRotation()
        {
            transform.up = Vector3.Lerp(transform.up, LegsAnim.LegsNormal, 0.5f);
        }
        private void SetBodyFlyRotation()
        {
            Vector3 direction = Vector3.up - Rig.velocity.normalized;
            transform.up = Vector3.Lerp(transform.up, direction, 0.05f);
        }

    }
}