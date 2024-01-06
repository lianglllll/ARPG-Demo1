using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [RequireComponent(typeof(Light))]
    public class IFX_LightAnimator : IFX_ControlledObject
    {
        public IFX_AnimationModule LightIntensity;

        public bool IsAutoStopEnabled = true;

        private float _startedTime;

        private Light _light;

        public override void Run()
        {
            base.Run();

            _light.gameObject.SetActive(true);
            _startedTime = Time.time;
            _light.intensity = LightIntensity.Evaluate(0);
        }

        public override void Stop()
        {
            base.Stop();

            _light.gameObject.SetActive(false);
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
            _light.intensity = LightIntensity.Evaluate(0);
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            var time = Time.time - _startedTime;
            var lightIntensity = LightIntensity.Evaluate(time);
            _light.intensity = lightIntensity;

            var lastKeyFrame = LightIntensity.AnimationCurve[LightIntensity.AnimationCurve.length - 1];
            var lastTime = lastKeyFrame.time;

            if (time >= lastTime && IsAutoStopEnabled)
                Stop();
        }
    }
}