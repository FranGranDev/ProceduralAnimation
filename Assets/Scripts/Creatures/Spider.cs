using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class Spider : Creature, IGadget
    {
        [SerializeField] private int maxHp;
        [SerializeField] private int hp;
        [SerializeField] private bool dead;

        [Range(0, 15f)] [SerializeField] private float Speed;
        [Range(0, 360f)] [SerializeField] private float RotationSpeed;
        [Range(0, 270f)] [SerializeField] private float AirRotationSpeed;
        [Range(0, 100f)] [SerializeField] private float JumpVelocity;

        [Range(0, 3f)] [SerializeField] private float MinHeight;
        [Range(0, 1f)] [SerializeField] private float BodyUpSpeed;

        [Header("Components")]
        [SerializeField] private GameObject weapon;
        [SerializeField] private IGadget gadget;
        [SerializeField] private Transform body;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private ProceduralAnimation legsAnim;

        public float CurrantBodyUpSpeed { get; private set; }
        public Vector3 Direction { get; private set; }
        public float CurrantSpeed { get; private set; }
        public override Transform Body => body;


        public override int Hp
        {
            get => hp;
            protected set
            {
                hp = value;
                if(hp <= 0)
                {
                    Die();
                }
            }
        }
        public override bool Dead => dead;
        public override int MaxHp => maxHp;

        public override void GetHit(Hit hit)
        {
            Hp -= hit.Damage;
        }
        public override void GetHit(ImpulseHit hit)
        {
            Hp -= hit.Damage;

            rig.AddForce(hit.Impulse, ForceMode.Impulse);
            rig.AddTorque(hit.Impulse, ForceMode.Impulse);
        }

        public override void Die()
        {
            dead = true;
            hp = 0;

            OnDie?.Invoke();

            Destroy(gameObject);
        }
        public override System.Action OnDie { get; set; }

        public override void Move(Vector3 direction, float rotation)
        {
            direction.y = 0f;

            switch(State)
            {
                case MoveState.OnGround:
                    body.Rotate(Vector3.up, rotation * RotationSpeed * Time.fixedDeltaTime);
                    rig.velocity = Vector3.Lerp(rig.velocity, body.TransformDirection(direction) * Speed, 0.25f);
                    CurrantSpeed = rig.velocity.magnitude / Speed;
                    break;
                case MoveState.Fly:
                    body.Rotate(Vector3.up, rotation * AirRotationSpeed * Time.fixedDeltaTime);
                    transform.Rotate(body.right, direction.z * AirRotationSpeed * Time.fixedDeltaTime);
                    CurrantSpeed = rig.velocity.magnitude / Speed;
                    break;
                case MoveState.Jumping:
                    body.Rotate(Vector3.up, rotation * AirRotationSpeed * Time.fixedDeltaTime);
                    transform.Rotate(body.right, direction.z * AirRotationSpeed * Time.fixedDeltaTime);
                    CurrantSpeed = rig.velocity.magnitude / Speed;
                    break;
            }
        } //Use direction in localSpace!
        public override void Jump()
        {
            if (State != MoveState.OnGround)
                return;
            State = MoveState.Fly;
            rig.AddForce(transform.up * JumpVelocity, ForceMode.VelocityChange);
            SetState(MoveState.Jumping);
            StartCoroutine(JumpDelayCour());
        }
        private IEnumerator JumpDelayCour()
        {
            yield return new WaitForSeconds(0.5f);
            while(legsAnim.LegsOnGround < legsAnim.Legs.Length / 2)
            {
                if (State == MoveState.Fly)
                    yield break;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(0.5f);
            SetState(MoveState.OnGround);
            yield break;
        }


        public void ActionPoint(Vector3 point)
        {
            if (gadget == null)
                return;
            gadget.ActionPoint(point);
        }
        public void OnActionStart(System.Action callback)
        {
            if (gadget == null)
                return;
            callback = null;
            //switch(Weapon.CallbackAction)
            //{
            //    case Gadget.CreatureActionType.Jump:
            //        callback = Jump;
            //        break;
            //}
            gadget.OnActionStart(callback);
        }
        public void OnActionEnd(IEntity self)
        {
            if (gadget == null)
                return;
            gadget.OnActionEnd(this);
        }
        public void ChangeGadget(bool right)
        {
            
        }


        public override void StateSelect()
        {
            switch (State)
            {
                case MoveState.Fly:
                    if (legsAnim.LegsOnGround >= legsAnim.Legs.Length - 1)
                    {
                        SetState(MoveState.OnGround);
                    }
                    break;
                case MoveState.OnGround:
                    if (legsAnim.LegsOnGround < legsAnim.Legs.Length - 1)
                    {
                        SetState(MoveState.Fly);
                    }
                    break;
                case MoveState.Jumping:
                    {
                        if (legsAnim.LegsOnGround == 0)
                        {
                            SetState(MoveState.Fly);
                        }
                    }
                    break;
            }
        }
        public override void StateExecute()
        {
            legsAnim.Simulate(Direction);

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
            gadget = weapon.GetComponent<IGadget>();
        }

        private void SetState(MoveState state)
        {
            if (State == state)
                return;
            State = state;
            switch (State)
            {
                case MoveState.Fly:
                    rig.useGravity = true;
                    break;
                case MoveState.OnGround:
                    rig.useGravity = false;
                    break;
                case MoveState.Jumping:
                    rig.useGravity = true;
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
                    rig.AddForce(-transform.up * CurrantBodyUpSpeed, ForceMode.VelocityChange);
                }
                else if (hit.distance < MinHeight * 0.9f)
                {
                    rig.AddForce(transform.up * CurrantBodyUpSpeed, ForceMode.VelocityChange);
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
            transform.up = Vector3.Lerp(transform.up, legsAnim.LegsNormal, 0.5f);
        }
        private void SetBodyFlyRotation()
        {
            Vector3 direction = Vector3.up - rig.velocity.normalized;
            transform.up = Vector3.Lerp(transform.up, direction, 0.05f);
        }
    }
}