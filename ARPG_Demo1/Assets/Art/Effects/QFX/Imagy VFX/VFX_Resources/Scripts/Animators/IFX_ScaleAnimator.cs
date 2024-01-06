using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_ScaleAnimator : IFX_ControlledObject
    {
        public IFX_AnimationModule ScaleAnimation;

        public bool IsAutoStopEnabled = true;

        private float _startedTime;

        public override void Run()
        {
            base.Run();

            _startedTime = Time.time;

            var scale = ScaleAnimation.Evaluate(0);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            var time = Time.time - _startedTime;

            var scale = ScaleAnimation.Evaluate(time);
            transform.localScale = new Vector3(scale, scale, scale);

            var lastKeyFrame = ScaleAnimation.AnimationCurve[ScaleAnimation.AnimationCurve.length - 1];
            var lastTime = lastKeyFrame.time;

            if (time > lastTime && IsAutoStopEnabled)
                Stop();
        }
    }
}
