using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Ai;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(ITouchMovement), typeof(UnitCollection))]
    public class SelectionMovement : MonoBehaviour
    {
        //Links
        private ITouchMovement movement;
        private UnitCollection collection;
        [SerializeField] private SelectBoxDrawer drawer;

        //State
        private List<ISelectable> selectedUnits = new List<ISelectable>();

        private void Awake()
        {
            movement = GetComponent<ITouchMovement>();
            collection = GetComponent<UnitCollection>();

            movement.OnClick += OnClick;
            movement.OnStartSelect += OnStartSelect;
            movement.OnEndSelect += OnEndSelect;
            movement.OnDragBox += OnDragBox;
        }

        private void OnClick(Vector2 position, ITouchMovement.MouseButtons button)
        {
            ISelectable unit = null;
            if (SelectRaycast.TryGetUnit(ref unit, position))
            {
                Debug.Log("da");

                if (button == ITouchMovement.MouseButtons.Right)
                {
                    //Unit Attack
                }
                else
                {
                    Debug.Log("da");
                    SelectSingleUnit(unit);
                }
            }
            else
            {
                if (button == ITouchMovement.MouseButtons.Right)
                {
                    UnselectAll();
                }
                else
                {
                    UnitsMove(position);
                }
            }
        }
        private void OnStartSelect(Vector2 position, ITouchMovement.MouseButtons button)
        {
            drawer.Create();
        }
        private void OnEndSelect(Vector2 first, Vector2 second, ITouchMovement.MouseButtons button)
        {
            drawer.Destroy();

            List<ISelectable> unitsToSelect = new List<ISelectable>();

            foreach(ISelectable unit in collection.Get)
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

                float left = Mathf.Min(first.x, second.x);
                float right = Mathf.Max(first.x, second.x);
                float bottom = Mathf.Min(first.y, second.y);
                float top = Mathf.Max(first.y, second.y);

                if(screenPos.x > left && screenPos.x < right &&
                   screenPos.y > bottom && screenPos.y < top)
                {
                    unitsToSelect.Add(unit);
                }
            }

            SelectUnits(unitsToSelect);
        }
        private void OnDragBox(Vector2 first, Vector2 second, ITouchMovement.MouseButtons button)
        {
            drawer.UpdateBox(first, second);
        }


        private void SelectUnits(List<ISelectable> units)
        {
            UnselectAll();

            foreach(ISelectable unit in units)
            {
                unit.Selected = true;
            }
            selectedUnits = units;
        }
        private void SelectSingleUnit(ISelectable unit)
        {
            UnselectAll();

            selectedUnits.Add(unit);
            unit.Selected = true;
        }
        private void UnselectAll()
        {
            foreach(ISelectable unit in selectedUnits)
            {
                unit.Selected = false;
            }
            selectedUnits.Clear();
        }

        private void UnitsMove(Vector2 position)
        {
            Vector3 worldPos = Vector3.zero;
            if(SelectRaycast.GetPoint(ref worldPos, position))
            {
                foreach(ISelectable unit in selectedUnits)
                {
                    unit.MoveTo(worldPos);
                }
            }
        }

        private void OnDrawGizmos()
        {

        }
    }
}
