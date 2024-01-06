using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_ShadowHoleAbilityController : MonoBehaviour, IFX_IActivableAnimatorAbility
    {
        public GameObject ShadowAura;
        public GameObject ShadowHole;

        public LineRenderer LineRenderer;
        public int BezierCurveSegmentCount;
        public CurveSettings[] Curves;

        public bool IsTargetMotionEnabled;
        public bool IsTargetDestroyEnabled;

        public float TargetMotionDelay;
        public float TargetMotionOffset;
        public float TargetMotionSpeed;

        public float TargetLifeTime;

        public AudioSource AudioSource;
        public AudioClip AudioClip;

        private readonly IFX_TargetAttacher _targetAttacher = new IFX_TargetAttacher();
        private GameObject _aura;

        private readonly List<LineRenderer> _lrs = new List<LineRenderer>();

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

        public void Launch()
        {
            if (_aura != null)
            {
                var auraScaleAnimation = _aura.GetComponent<IFX_ScaleAnimator>();
                if (auraScaleAnimation != null)
                    auraScaleAnimation.Run();
            }

            _aura = Instantiate(ShadowAura);
            _aura.transform.position = Emitter.transform.position;
            _aura.transform.parent = transform;

            transform.position = new Vector3(Emitter.transform.position.x, 0, Emitter.transform.position.y);
            _targetAttacher.Attach(Emitter.transform, transform);

            if (AudioSource != null && AudioClip != null)
                AudioSource.PlayOneShot(AudioClip);

        }

        public void Activate()
        {
            CreateCurves(TargetPosition);

            var shadowHole = Instantiate(ShadowHole);
            shadowHole.transform.position = TargetPosition;

            if (Target != null)
                IFX_InvokeUtil.RunLater(this, AddTargetComponents, TargetMotionDelay);
        }

        private void AddTargetComponents()
        {
            if (IsTargetDestroyEnabled)
            {
                var targetSelfDestroyer = Target.gameObject.AddComponent<IFX_SelfDestroyer>();
                targetSelfDestroyer.LifeTime = TargetLifeTime;
                targetSelfDestroyer.Run();
            }

            if (IsTargetMotionEnabled)
            {

                var targetLerpMotion = Target.gameObject.AddComponent<IFX_LerpMotion>();
                targetLerpMotion.LaunchPosition = Target.transform.position;
                targetLerpMotion.TargetPosition = Target.transform.position + Vector3.down * TargetMotionOffset;
                targetLerpMotion.Speed = TargetMotionSpeed;
                targetLerpMotion.Run();
            }
        }

        private void CreateCurves(Vector3 endPosition)
        {
            foreach (var curveSpellSettings in Curves)
            {
                var lr = Instantiate(LineRenderer).GetComponent<LineRenderer>();

                lr.gameObject.SetActive(false);
                lr.transform.parent = transform.root;
                lr.enabled = false;

                SetLineRendererPositions(lr, curveSpellSettings, endPosition);

                _lrs.Add(lr);
            }
        }

        private void SetLineRendererPositions(LineRenderer lr, CurveSettings curveSettings, Vector3 endPosition)
        {
            var lastPoint = curveSettings.ControlPoints[curveSettings.ControlPoints.Length - 1];

            var scaleFactor = Vector3.Distance(lastPoint.position, endPosition);
            var dir = (endPosition - lastPoint.position).normalized;

            for (int i = 1; i < curveSettings.ControlPoints.Length; i++)
                curveSettings.ControlPoints[i].position = curveSettings.ControlPoints[i].position + dir * scaleFactor;

            lastPoint.position = TargetPosition;

            var bezierCurve = IFX_BezierHelper.GetBezierCurve(curveSettings.ControlPoints, BezierCurveSegmentCount);
            lr.positionCount = bezierCurve.PositionCount;
            lr.SetPositions(bezierCurve.Positions);
            lr.SetPosition(0, curveSettings.ControlPoints[0].position);
            lr.enabled = true;
            lr.gameObject.SetActive(true);
        }

        private void Update()
        {
            _targetAttacher.Update();
        }

        private void OnDestroy()
        {
            foreach (var lr in _lrs)
                Destroy(lr.gameObject);
            _lrs.Clear();
        }

        [Serializable]
        public class CurveSettings
        {
            public Transform[] ControlPoints;
        }
    }
}
