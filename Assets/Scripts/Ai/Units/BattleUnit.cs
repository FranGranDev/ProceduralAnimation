using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{

    [RequireComponent(typeof(IEntity), typeof(IMovement), typeof(IGadget))]
    public class BattleUnit : Unit, ISelectable
    {
        //Components
        [SerializeField] private SelectColorChanger colorChanger;

        //------------Links--------------
        private IMovement movement;
        private IGadget gadget;
        private IEntity entity;

        private IUnitBehavior behavior;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                if(selected)
                {
                    colorChanger.SetSelectedColor();
                    //Debug.Log("Selected: " + gameObject.name, gameObject);
                }
                else
                {
                    colorChanger.SetBaseColor();
                    //Debug.Log("Unselected: " + gameObject.name, gameObject);
                }
            }
        }
        private bool selected;

        public System.Action OnDisable
        {
            get => entity.OnDie;
            set => entity.OnDie = value;
        }

        private IEntity currantEnemy;
        private Stack<IEntity> enemys = new Stack<IEntity>();
        private Vector3 targetPoint;
        //----------------------------

        private void Awake()
        {
            Initilize();
        }
        private void Start()
        {
            State = States.Idle;
        }
        

        public virtual void Initilize()
        {
            movement = GetComponent<IMovement>();
            gadget = GetComponent<IGadget>();
            entity = GetComponent<IEntity>();

            behavior = new BattleUnitBehavior(movement, gadget, SetState, this);
        }

        
        #region Commands
        public void MoveTo(Vector3 position)
        {
            State = States.Move;
            targetPoint = position;
        }
        public void Attack(List<IEntity> enities)
        {
            State = States.Attack;
            enemys.Clear();
            foreach (IEntity enemy in enities)
            {
                if (!enemys.Contains(enemy))
                {
                    enemys.Push(enemy);
                }
            }

            if (currantEnemy != null)
            {
                currantEnemy.OnDie -= AttackNext;
            }
            if(enemys.Count > 0)
            {
                currantEnemy = enemys.Pop();
                currantEnemy.OnDie += AttackNext;
            }
        }
        public void Attack(IEntity enemy)
        {
            State = States.Attack;
            if(!enemys.Contains(enemy))
            {
                enemys.Push(enemy);
            }

            if (currantEnemy != null)
            {
                currantEnemy.OnDie -= AttackNext;
            }
            if (enemys.Count > 0)
            {
                currantEnemy = enemys.Pop();
                currantEnemy.OnDie += AttackNext;
            }

            
        }
        private void AttackNext()
        {
            if (currantEnemy != null)
            {
                currantEnemy.OnDie -= AttackNext;
            }
            if(State == States.Attack && enemys.Count > 0)
            {
                Attack(enemys.Pop());
            }
            else
            {
                SetState(States.Idle);
            }
        }

        public override void OnStateStart(States state)
        {
            
        }
        public override void OnStateEnd(States state)
        {
            switch(state)
            {
                case States.Attack:
                    enemys.Clear();
                    if(currantEnemy != null)
                    {
                        currantEnemy.OnDie -= AttackNext;
                    }
                    currantEnemy = null;
                    break;
            }
        }
        #endregion

        #region Interact
        public void OnSendAccept()
        {
            enemys.Clear();
        }
        public void Accept(ISelectable other)
        {
            other.Attack(entity);
        }
        public void Accept(List<ISelectable> others)
        {
            foreach(ISelectable unit in others)
            {
                unit.Attack(entity);
            }
        }
        public void OnAcceptStarted()
        {

        }
        public void OnAcceptEnded()
        {

        }
        #endregion

        #region Squad
        public void Add(ISelectable unit)
        {
            Debug.Log("Cant add to unit");
        }
        public void Clear()
        {

        }
        #endregion

        #region Movement

        public override void Moving()
        {
            behavior.Moving(targetPoint);
        }
        public override void Staying()
        {
            behavior.Stay();
        }
        public override void Attacking()
        {
            behavior.Attack(currantEnemy);
        }

        #endregion

    }
}
