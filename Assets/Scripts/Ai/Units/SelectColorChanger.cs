using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ai
{
    public class SelectColorChanger : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> renderers;
        [SerializeField] private Color baseColor;
        [SerializeField] private Color selectedColor;

        public void SetBaseColor()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = baseColor;
            }
        }
        public void SetSelectedColor()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = selectedColor;
            }
        }

        private void OnValidate()
        {
            //SetBaseColor();
        }
    }
}
