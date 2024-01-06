using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_PaperPowerAbilityController : MonoBehaviour, IFX_IActivableAnimatorAbility
    {
        public Transform ActivateTransformParent;
        public Transform ActivateTransform;

        public GameObject LineRenderer;
        public GameObject LaunchPs;
        public Transform LaunchPsTransform;
        public GameObject HitPs;
        public float LaunchPsDelay;
        public float HitPsDelay;

        public int BezierCurveSegmentCount;

        public CurveSettings[] Curves;
        public float InstantiateCurveDelay;

        public string LaunchAnchorName;

        public IFX_LerpMotion LerpMotion;

        public ParticleSystem MotionFinishedPs;

        public Transform PaperPowerSelfTransform;

        public AudioSource AudioSource;
        public bool PlayLaunchCurveAudioOnce;
        public AudioClip LaunchCurveAudioClip;

        private readonly IFX_TargetAttacher _ifxTargetAttacher = new IFX_TargetAttacher();

        private ParticleSystem _launchPs;
        private ParticleSystem _hitPs;
        private readonly List<LineRenderer> _lrs = new List<LineRenderer>();
        private ParticleSystem _motionFinishedPs;

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }
        public Vector3 TargetRotation { get; set; }

        public void Launch()
        {
            _ifxTargetAttacher.FindAndAttach(LaunchAnchorName, Emitter, PaperPowerSelfTransform);

            ActivateTransformParent.rotation = Emitter.root.rotation;
            ActivateTransformParent.position = Emitter.position;
        }

        public void Activate()
        {
            _ifxTargetAttacher.DeAttach();

            LerpMotion.SelfTransform = PaperPowerSelfTransform;
            LerpMotion.LaunchPosition = PaperPowerSelfTransform.position;
            LerpMotion.TargetPosition = ActivateTransform.position;
            LerpMotion.TargetRotation = ActivateTransform.rotation;

            LerpMotion.Run();
        }

        private void MotionFinished()
        {
            if (_motionFinishedPs == null)
                return;

            _motionFinishedPs.transform.rotation = ActivateTransform.rotation;
            _motionFinishedPs.transform.position = ActivateTransform.position;
            _motionFinishedPs.Play();

            _lrs.Clear();

            StartCoroutine("CastSpell", TargetPosition);
        }

        private IEnumerator CastSpell(Vector3 endPosition)
        {
            _launchPs.transform.position = LaunchPsTransform.position;
            _launchPs.transform.rotation = LaunchPsTransform.rotation;

            _hitPs.transform.position = TargetPosition;
            _hitPs.transform.rotation = Quaternion.FromToRotation(_hitPs.transform.up, TargetRotation) *
                                        _hitPs.transform.rotation;

            if (PlayLaunchCurveAudioOnce)
                PlayLaunchCurveAudio();

            foreach (var curveSpellSettings in Curves)
            {
                var lr = Instantiate(LineRenderer).GetComponent<LineRenderer>();

                lr.gameObject.SetActive(false);
                lr.transform.parent = transform.root;
                lr.enabled = false;

                SetLineRendererPositions(lr, curveSpellSettings, endPosition);

                _lrs.Add(lr);

                IFX_InvokeUtil.RunLater(this, delegate
                {
                    _launchPs.Play();
                    if (!PlayLaunchCurveAudioOnce)
                        PlayLaunchCurveAudio();

                }, LaunchPsDelay);

                IFX_InvokeUtil.RunLater(this, () => _hitPs.Play(), HitPsDelay);

                yield return new WaitForSeconds(InstantiateCurveDelay);
            }
        }

        private void PlayLaunchCurveAudio()
        {
            if (AudioSource != null && LaunchCurveAudioClip != null)
                AudioSource.PlayOneShot(LaunchCurveAudioClip);
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

        private void Awake()
        {
            _launchPs = Instantiate(LaunchPs).GetComponent<ParticleSystem>();
            _launchPs.transform.parent = transform.root;
            _hitPs = Instantiate(HitPs).GetComponent<ParticleSystem>();
            _hitPs.transform.parent = transform.root;

            if (MotionFinishedPs != null)
                _motionFinishedPs = Instantiate(MotionFinishedPs);

            LerpMotion.MotionFinished += MotionFinished;
        }

        private void Update()
        {
            _ifxTargetAttacher.Update();
        }

        [Serializable]
        public class CurveSettings
        {
            public Transform[] ControlPoints;
        }
    }
}