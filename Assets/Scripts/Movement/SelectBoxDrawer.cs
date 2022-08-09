using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Image))]
    public class SelectBoxDrawer : MonoBehaviour
    {
        private Image box;

        private void Awake()
        {
            box = GetComponent<Image>();
        }

        public void Create()
        {
            box.enabled = true;
            box.rectTransform.sizeDelta = Vector2.zero;
        }
        public void UpdateBox(Vector2 firstPoint, Vector2 secondPoint)
        {
            Vector2 center = (firstPoint + secondPoint) / 2;
            Vector2 size = new Vector2(Mathf.Abs(firstPoint.x - secondPoint.x), Mathf.Abs(firstPoint.y - secondPoint.y));

            Rect rect = new Rect(center, size);
            box.rectTransform.position = center;
            box.rectTransform.sizeDelta = size;
        }
        public void Destroy()
        {
            box.enabled = false;
        }
    }
}
