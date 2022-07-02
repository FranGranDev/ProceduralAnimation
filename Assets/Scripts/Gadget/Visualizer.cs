using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gadgets
{
    public class Visualizer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float DistanceStep;

        [Header("Components")]
        [SerializeField] private LineRenderer Line;
        public void DrawLine(Vector3 from, Vector3 to, AnimationCurve curve, float flyHeight)
        {
            float distance = (from - to).magnitude;
            int count = Mathf.FloorToInt(distance / DistanceStep);

            Line.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                float ratio = (float)i / (float)count;
                Vector3 upDirection = transform.up * Vector3.Dot(transform.up, Vector3.up);
                Vector3 height = upDirection * curve.Evaluate(ratio) * flyHeight;
                Vector3 point = Vector3.Lerp(from, to, ratio);
                Line.SetPosition(i, point + height);
            }
        }
        public void ClearLine()
        {
            Line.positionCount = 0;
        }
    }

    
}
