using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_WaterSplashAbilityController : MonoBehaviour, IFX_IActivableAnimatorAbility
    {
        public ParticleSystem CastingPs;
        public ParticleSystem TargetPs;

        public TrailRenderer TrailRenderer;
        public string RightHandAnchorName;
        public string LeftHandAnchorName;

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

        private readonly IFX_TargetAttacher _leftTargetAttacher = new IFX_TargetAttacher();
        private readonly IFX_TargetAttacher _righTargetAttacher = new IFX_TargetAttacher();
        private ParticleSystem _castingPs;
        private ParticleSystem _targetPs;
        private TrailRenderer _trLeft;
        private TrailRenderer _trRight;

        private Vector3 ResultTargetPosition
        {
            get
            {
                if (Target == null)
                    return TargetPosition;

                var coll = Target.GetComponent<Collider>();
                if (coll != null)
                    return coll.bounds.center;

                return Target.transform.position;
            }
        }

        public void Launch()
        {
            _leftTargetAttacher.FindAndAttach(LeftHandAnchorName, Emitter.root, _trLeft.gameObject.transform);
            _righTargetAttacher.FindAndAttach(RightHandAnchorName, Emitter.root, _trRight.gameObject.transform);

            _trLeft.Clear();
            _trRight.Clear();

            _leftTargetAttacher.Update();
            _righTargetAttacher.Update();

            _trLeft.enabled = true;
            _trRight.enabled = true;
        }

        public void Activate()
        {
            _castingPs = Instantiate(CastingPs);
            _castingPs.transform.position = Emitter.position;
            var groundAttacher = _castingPs.GetComponent<IFX_GroundAttacher>();
            if (groundAttacher != null)
                groundAttacher.Run();

            var castingPs = _castingPs.GetComponent<ParticleSystem>();
            if (castingPs != null)
                castingPs.Play();

            _targetPs = Instantiate(TargetPs);
            _targetPs.transform.position = ResultTargetPosition;
            _targetPs.Play();

            _leftTargetAttacher.DeAttach();
            _righTargetAttacher.DeAttach();
        }

        private void OnEnable()
        {

            _trLeft = Instantiate(TrailRenderer);
            _trRight = Instantiate(TrailRenderer);

            _trLeft.enabled = false;
            _trRight.enabled = false;
        }

        private void Update()
        {
            _leftTargetAttacher.Update();
            _righTargetAttacher.Update();
        }
    }
}
