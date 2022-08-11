using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public class Squad : MonoBehaviour, ISelectable
    {
        [SerializeField] private List<GameObject> startUnits;
        [SerializeField] private Transform flag;

        private List<ISelectable> units = new List<ISelectable>();

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                flag.gameObject.SetActive(selected);
                foreach (ISelectable unit in units)
                {
                    unit.Selected = value;
                }
            }
        }
        private bool selected;

        public System.Action OnDisable { get; set; }

        private Vector3 Center
        {
            get
            {
                if (units.Count == 0)
                    return lastCenter;
                Vector3 center = Vector3.zero;
                foreach(ISelectable unit in units)
                {
                    center += unit.transform.position;
                }
                lastCenter = center / units.Count;
                return lastCenter;
            }
        }
        private Vector3 lastCenter;

        private List<IEntity> enemys = new List<IEntity>();

        private void Start()
        {
            lastCenter = transform.position;
            foreach(GameObject unitObj in startUnits)
            {
                ISelectable unit = null;
                if(unitObj.TryGetComponent(out unit))
                {
                    units.Add(unit);
                }
            }
        }

        public void OnSendAccept()
        {
            enemys.Clear();
            foreach(ISelectable unit in units)
            {
                unit.OnSendAccept();
            }
        }
        public void Accept(ISelectable other)
        {
            enemys.Clear();

            foreach (ISelectable unit in units)
            {
                unit.Accept(other);
            }
        }
        public void Accept(List<ISelectable> others)
        {
            enemys.Clear();

            foreach (ISelectable other in others)
            {                
                foreach (ISelectable unit in units)
                {
                    unit.Accept(other);
                }
            }
        }


        public void MoveTo(Vector3 position)
        {
            enemys.Clear();

            foreach (ISelectable unit in units)
            {
                unit.MoveTo(position);
            }
        }
        public void Attack(IEntity entity)
        {
            if(!enemys.Contains(entity))
            {
                enemys.Add(entity);
            }

            foreach (ISelectable unit in units)
            {
                unit.Attack(enemys);
                enemys.Shuffle();
            }
        }
        public void Attack(List<IEntity> entity)
        {
            foreach (IEntity enemy in entity)
            {
                foreach (ISelectable unit in units)
                {
                    unit.Attack(entity);
                }
            }
        }

        public void Add(List<ISelectable> units)
        {
            units.AddRange(units);
        }
        public void Clear()
        {
            units.Clear();
        }


        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, Center, 0.1f);
        }
    }
}
