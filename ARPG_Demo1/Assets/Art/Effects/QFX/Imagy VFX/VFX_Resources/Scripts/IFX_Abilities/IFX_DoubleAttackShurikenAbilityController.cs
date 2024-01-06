using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_DoubleAttackShurikenAbilityController : MonoBehaviour, IFX_IActivableAnimatorAbility
    {
        public string AttachAnchorName;

        public ParticleSystem ShurikenPs;
        public ParticleSystem ThrowPs;
        public ParticleSystem MotionPs;

        public float ShurikenMotionSpeed;
        public float ShurikenMotionRotationSpeed;

        public float TargetOffset;
        public float ReturnDistance;

        public float DissapearDistance;
        public IFX_ShaderAnimator TrailShaderAnimator;
        public IFX_ShaderAnimator ShurikenShaderAnimator;

        public float PositionY = 1;

        public AudioSource AudioSource;
        public AudioClip AudioClip;

        private ParticleSystem _shurikenPs;
        private ParticleSystem _throwPs;
        private ParticleSystem _motionPs;
        private readonly IFX_TargetAttacher _targetAttacher = new IFX_TargetAttacher();

        private bool _isTargetReached;
        private bool _isTrailDissapearing;

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

        private Vector3 ResultTargetPosition
        {
            get
            {
                if (Target == null)
                    return new Vector3(TargetPosition.x, PositionY, TargetPosition.z);

                var coll = Target.GetComponent<Collider>();
                if (coll != null)
                    return coll.bounds.center;

                return Target.transform.position;

            }
        }

        public void Launch()
        {
            _shurikenPs = Instantiate(ShurikenPs);
            _shurikenPs.transform.parent = transform;
            _shurikenPs.transform.position = Vector3.zero;
            _shurikenPs.Play();

            _throwPs = Instantiate(ThrowPs);
            _throwPs.transform.parent = _shurikenPs.transform;
            _throwPs.transform.position = Vector3.zero;

            _targetAttacher.FindAndAttach(AttachAnchorName, Emitter.root, _shurikenPs.transform);
            _targetAttacher.Update();

            _motionPs = Instantiate(MotionPs, _shurikenPs.transform.position, Quaternion.identity, _shurikenPs.transform);
            _motionPs.Play();
        }

        public void Activate()
        {
            _targetAttacher.DeAttach();

            var rotationModule = _shurikenPs.rotationOverLifetime;
            rotationModule.enabled = true;

            TrailShaderAnimator.Target = _shurikenPs.GetComponent<IFX_HorizontalTrail>().TrailObject;
            TrailShaderAnimator.Setup();

            ShurikenShaderAnimator.Target = _shurikenPs.transform;
            ShurikenShaderAnimator.Setup();

            _throwPs.transform.position = _shurikenPs.transform.position;
            _throwPs.Play(true);

            _motionPs.Stop();

            _shurikenPs.transform.rotation = Quaternion.LookRotation(ResultTargetPosition - _shurikenPs.transform.position);

            if (AudioSource != null && AudioClip != null)
                AudioSource.PlayOneShot(AudioClip);
        }

        private void LateUpdate()
        {
            if (_targetAttacher.IsAttached)
            {
                _targetAttacher.Update();
                return;
            }

            var emitterAnchor = _targetAttacher.Target.position;

            var targetPos = _isTargetReached ? emitterAnchor : ResultTargetPosition;
            var emitterPos = _isTargetReached ? ResultTargetPosition : emitterAnchor;

            var targetOffset = (targetPos - emitterPos).normalized * TargetOffset;
            targetPos += targetOffset;

            _shurikenPs.transform.rotation = Quaternion.Slerp(_shurikenPs.transform.rotation,
                Quaternion.LookRotation(targetPos - _shurikenPs.transform.position), ShurikenMotionRotationSpeed * Time.deltaTime);

            _shurikenPs.transform.position += _shurikenPs.transform.forward * Time.deltaTime * ShurikenMotionSpeed;

            if (!_isTargetReached)
            {
                var distance = Vector3.Distance(_shurikenPs.transform.position, targetPos);
                if (distance <= ReturnDistance)
                    _isTargetReached = true;
            }
            else if (!_isTrailDissapearing)
            {
                var distance = Vector3.Distance(_shurikenPs.transform.position, emitterAnchor);
                if (distance <= DissapearDistance)
                {
                    TrailShaderAnimator.Run();
                    ShurikenShaderAnimator.Run();
                    _isTrailDissapearing = true;
                }
            }
        }
    }
}