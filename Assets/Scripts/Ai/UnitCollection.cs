using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public class UnitCollection : MonoBehaviour
    {
        public int Count;
        [SerializeField] private List<ISelectable> allUnits;
        public List<ISelectable> Get => allUnits;

        private void Start()
        {
            allUnits = new List<ISelectable>(FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>());
            foreach(ISelectable unit in allUnits)
            {
                unit.OnDisable += () => { allUnits.Remove(unit); };
            }
            Count = allUnits.Count;
        }
    }
}
