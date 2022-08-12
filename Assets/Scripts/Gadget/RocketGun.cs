using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gadgets
{
    public class RocketGun : Gadget
    {
        [Header("Settings")]
        [SerializeField] private AnimationCurve FlyCurve;
        [SerializeField] private float FlyHeight;
        [SerializeField] private float BulletSpeed;
        [SerializeField] private float ReloadTime;
        [SerializeField] private bool Visualize;

        [Header("Components")]
        [SerializeField] private Transform FirePoint;
        [SerializeField] private Visualizer visualizer;
        [SerializeField] private Bullet bullet;

        private Vector3 CurrantPoint;
        private bool ActionStarted;

        private bool Reloaded = true;
        private bool Reloading;

        public override void OnActionEnd(IEntity ignore)
        {
            Fire(ignore);
            ActionStarted = false;


            if (Visualize)
            {
                visualizer.ClearLine();
            }
        }
        public override void OnActionStart(System.Action callback)
        {
            ActionStarted = true;
        }
        public override void ActionPoint(Vector3 point)
        {
            CurrantPoint = point;
        }

        private void LateUpdate()
        {
            if (ActionStarted && Visualize)
            {
                visualizer.DrawLine(FirePoint.position, CurrantPoint, FlyCurve, FlyHeight);
            }
        }

        private void Fire(IEntity ignore)
        {
            if (Reloaded)
            {
                Vector3 upDirection = transform.up * Vector3.Dot(transform.up, Vector3.up);
                BulletData data = new BulletData(FlyCurve, FlyHeight, CurrantPoint, transform.position, upDirection, BulletSpeed, ignore);
                Instantiate(bullet, FirePoint.position, FirePoint.rotation, null).Fire(data);

                StartCoroutine(ReloadCour());
            }
            else if(!Reloading)
            {
                StartCoroutine(ReloadCour());
            }
        }
        private IEnumerator ReloadCour()
        {
            Reloaded = false;
            Reloading = true;
            yield return new WaitForSeconds(ReloadTime);
            Reloaded = true;
            Reloading = false;
            yield break;
        }


        public override void Init()
        {
            if (visualizer == null)
                Visualize = false;
        }
    }
}