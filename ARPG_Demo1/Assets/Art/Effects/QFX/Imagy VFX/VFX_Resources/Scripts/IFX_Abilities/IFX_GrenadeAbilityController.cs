using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_GrenadeAbilityController : MonoBehaviour, IFX_IActivableAnimatorAbility
    {
        public Transform GrenadeFx;
        public Transform GrenadeMesh;
        public string FxAnchorName;
        public Vector3 FxAnchorRotation;
        public Vector3 FxAnchorOffset;

        public float ExplodeDistance;
        public ParticleSystem ExplosionPs;

        public TrailRenderer TrailRenderer;
        public IFX_ShaderAnimator TrailShaderAnimator;

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

        public float Speed;
        public float Height;
        public float Turn;

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

        //private GameObject _grenade;
        private readonly IFX_TargetAttacher _targetAttacher = new IFX_TargetAttacher();
        private IFX_LerpMotion _lerpMotion;
        private ParticleSystem _explosionPs;
        private bool _isActivated;
        private bool _isExploded;

        public void Launch()
        {
            //_grenade = Instantiate(GrenadeFx);
            //_grenade.transform.parent = transform;

            _targetAttacher.Offset = FxAnchorOffset;
            _targetAttacher.RotationOffset = FxAnchorRotation;
            _targetAttacher.FindAndAttach(FxAnchorName, Emitter.root, GrenadeFx.transform);
            _targetAttacher.Update();

            _lerpMotion = GrenadeFx.gameObject.AddComponent<IFX_LerpMotion>();
            _lerpMotion.Speed = Speed;
            _lerpMotion.ArcMotionHeight = Height;
            _lerpMotion.IsArcMotionEnabled = true;
            _lerpMotion.TargetRotation = Quaternion.identity;
            _lerpMotion.ChangeRotation = true;
            _lerpMotion.Turn = Turn;
        }

        public void Activate()
        {
            _targetAttacher.DeAttach();
            _lerpMotion.LaunchPosition = GrenadeFx.transform.position;
            _lerpMotion.TargetPosition = ResultTargetPosition;
            _lerpMotion.Run();
            _isActivated = true;    
            TrailRenderer.enabled = true;
        }

        private void OnEnable()
        {
            TrailRenderer.enabled = false;
        }

        private void LateUpdate()
        {
            if (_targetAttacher.IsAttached)
                _targetAttacher.Update();

            if (_isActivated && !_isExploded)
            {
                var distance = Vector3.Distance(GrenadeFx.transform.position, _lerpMotion.TargetPosition);
                if (distance <= ExplodeDistance)
                {
                    GrenadeMesh.gameObject.SetActive(false);
                    _explosionPs = Instantiate(ExplosionPs, Vector3.zero, Quaternion.identity, transform);
                    _explosionPs.transform.position = GrenadeFx.transform.position;
                    _explosionPs.Play();
                    _lerpMotion.Stop();
                    _isExploded = true;
                    TrailShaderAnimator.Run();
                }
            }
        }
    }
}
