using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Creatures;

namespace Assets.Scripts.Ai
{
    delegate void UnitAction();

    [RequireComponent(typeof(IMovement))]
    public class Unit : MonoBehaviour, ISelectable
    {
        //Components
        [SerializeField] private List<MeshRenderer> renderers;
        [SerializeField] private Color baseColor;
        [SerializeField] private Color selectedColor;

        //Links
        private IMovement creature;

        //Actions
        private UnitAction fixedUpdateAction;

        //States
        [SerializeField] private float rotation;
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
                    foreach(Renderer renderer in renderers)
                    {
                        renderer.material.color = selectedColor;
                    }
                    Debug.Log("Selected: " + gameObject.name, gameObject);
                }
                else
                {
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.material.color = baseColor;
                    }
                    Debug.Log("Unselected: " + gameObject.name, gameObject);
                }
            }
        }

        private bool moving;
        private Vector3 targetPoint;
        private bool selected;
        private Coroutine moveCoroutine;

        private void Awake()
        {
            creature = GetComponent<Creature>();
        }
        private void Start()
        {
            fixedUpdateAction += Stay;
        }

        public void MoveTo(Vector3 position)
        {
            if(!moving)
            {
                fixedUpdateAction += Moving;
                fixedUpdateAction -= Stay;
            }
            targetPoint = position;
            Debug.Log(position);
            moving = true;
        }
        public void Add(List<ISelectable> units)
        {
            Debug.Log("Cant add to unit");
        }
        public void Clear()
        {

        }

        private void Moving()
        {
            if (Vector3.Distance(creature.Body.position, targetPoint) < 2)
            {
                fixedUpdateAction -= Moving;
                fixedUpdateAction += Stay;
                moving = false;
                return;
            }
            Vector3 direction = (targetPoint - creature.Body.position).normalized;
            rotation = Mathf.Clamp(5 * Vector3.Dot(direction, creature.Body.right), -1, 1);
            direction = Vector3.forward * (1 - Mathf.Abs(rotation) * 0.25f);

            creature.Move(direction, rotation);
        }
        private void Stay()
        {
            creature.Move(Vector3.zero, 0);
        }

        private void FixedUpdate()
        {
            fixedUpdateAction?.Invoke();
        }
    }
}
