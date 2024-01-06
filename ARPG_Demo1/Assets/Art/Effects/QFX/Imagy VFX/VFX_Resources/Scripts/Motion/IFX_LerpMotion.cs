using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_LerpMotion : IFX_ControlledObject
    {
        public Transform SelfTransform;

        public float Speed = 1f;
        public float Turn = 1f;

        public bool ChangeRotation;
        public bool LookAtTarget;

        public bool IsArcMotionEnabled;
        public float ArcMotionHeight;

        private bool _isLerping;
        private float _timeStartedLerping;

        private Transform _transform;

        public Vector3 LaunchPosition { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform TargetTransform { get; set; }
        public Quaternion TargetRotation { get; set; }

        public Action MotionFinished;

        public override void Run()
        {
            _isLerping = true;
            _timeStartedLerping = Time.time;

            _transform = SelfTransform != null ? SelfTransform : transform;

            base.Run();
        }

        private void FixedUpdate()
        {
            if (!IsRunning || !_isLerping)
                return;

            var timeSinceStarted = Time.time - _timeStartedLerping;
            var percentageComplete = timeSinceStarted / Speed;

            var nextStep = Vector3.Lerp(LaunchPosition, TargetPosition, percentageComplete);

            _transform.position = !IsArcMotionEnabled ? nextStep : GetArcMotionPos(LaunchPosition, nextStep, TargetPosition, ArcMotionHeight);

            if (ChangeRotation)
            {
                Quaternion targetRotation;

                if (LookAtTarget)
                {
                    var direction = TargetTransform.position - _transform.position;
                    targetRotation = Quaternion.LookRotation(direction);
                }
                else
                {
                    targetRotation = TargetRotation;
                }

                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, Turn);
            }

            if (percentageComplete > 1.0f)
            {
                _isLerping = false;
                if (MotionFinished != null)
                    MotionFinished.Invoke();
            }
        }

        private static Vector3 GetArcMotionPos(Vector3 startPosition, Vector3 nextPos, Vector3 targetPosition, float arcHeight)
        {
            var x0 = startPosition.x;
            var x1 = targetPosition.x;
            var dist = x1 - x0;
            var nextX = nextPos.x;
            var nextZ = nextPos.z;
            var baseY = Mathf.Lerp(startPosition.y, targetPosition.y, (nextX - x0) / dist);
            var arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            return new Vector3(nextX, baseY + arc, nextZ);
        }
    }
}