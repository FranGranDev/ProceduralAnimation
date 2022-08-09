using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public class Squad : MonoBehaviour, ISelectable
    {
        [SerializeField] private List<GameObject> startUnits;
        private List<ISelectable> units = new List<ISelectable>();

        [SerializeField] private Transform flag;

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
                foreach(ISelectable unit in units)
                {
                    unit.Selected = value;
                }
            }
        }
        private bool selected;

        public void MoveTo(Vector3 position)
        {
            int coln = Mathf.CeilToInt(Mathf.Sqrt(units.Count));
            int row = Mathf.RoundToInt(units.Count / coln);

            int unitIndex = 0;

            Debug.Log("Count: " + units.Count + " Coln: " + coln + " Row: " + row);

            for(int y = -coln / 2; y < coln / 2 || unitIndex < units.Count; y++)
            {
                for (int x = -row / 2; x < row / 2; x++)
                {
                    if (unitIndex >= units.Count)
                        break;
                    Vector3 offset = new Vector3(x, y, 0);

                    units[unitIndex].MoveTo(position + offset);
                    unitIndex++;
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
