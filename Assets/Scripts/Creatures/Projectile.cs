using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class Projectile : MonoBehaviour
    {
        public Vector3 Normal { get; private set; }
        [SerializeField] private Transform Base;

        [SerializeField] private float VerticalLenght;
        [SerializeField] private float HorizontalLenght;

        public static Vector3 Project(Vector3 forward, Transform transform)
        {
            return transform.TransformDirection(forward);
        }
        public Vector3 Project(Vector3 forward)
        {
            return forward - Vector3.Dot(forward, Normal) * Normal;
        }
        
        private void UpdateNormal()
        {
            Vector3 normal = Vector3.zero;
            Vector3[] RayVectors = new Vector3[5] { -transform.up * VerticalLenght, transform.forward * HorizontalLenght, -transform.forward * HorizontalLenght, transform.right * HorizontalLenght, -transform.right * HorizontalLenght };
            for (int i = 0; i < RayVectors.Length; i++)
            {
                RaycastHit temp;
                if (Physics.Raycast(transform.position, RayVectors[i], out temp, RayVectors[i].magnitude, 1 << 3))
                {
                    normal += temp.normal;
                }
            }
            Normal = normal.normalized;
        }

        private void Update()
        {
            UpdateNormal();

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + Normal * 2);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Project(transform.forward) * 2);
        }
    }
}