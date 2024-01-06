using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_BlinkController : MonoBehaviour
    {
        public float BlinkOffset;

        public LineRenderer LineRenderer;
        public Transform LineRendererAnchor;
        public int LineRendererPositionCount;

        public IFX_AnimationModule LineRendererWidthAnimation;

        public GameObject BlinkLaunchFx;
        public GameObject BlinkLaunchFxAnchor;

        public GameObject BlinkLandingFx;
        public GameObject BlinkLandingFxAnchor;

        public AudioClip BlinkAudioClip;
        public AudioSource AudioSource;

        [Header("Material Adder")]
        public bool AddMaterial;
        public Material Material;
        public float MaterialLifeTime;
        public GameObject[] MaterialTargets;

        [Header("Shader Animator")]
        public bool AddShaderAnimator;
        public IFX_DynamicShaderParameter DynamicShaderParameter;

        [Header("Cloner")]
        public bool MakeClone;
        public Transform CloneSpecificTarget;
        public IFX_AnimationModule CloneOpacityAnimation;
        public float CloneLifeTime;
        public Material CloneMaterial;
        public Component[] CloneComponentsWithRequirements;

        private GameObject _blinkLaunchGo;
        private ParticleSystem _blinkLaunchPs;

        private GameObject _blinkLandingGo;
        private ParticleSystem _blinkLandingPs;

        private LineRenderer _lr;

        private float _blinkTime;

        private Vector3[] _points;
        private int _pointsCount;
        private bool _isRunning;

        private IFX_MaterialAdder _materialAdder;
        private IFX_ShaderAnimator _shaderAnimator;

        public void Blink()
        {
            _blinkLaunchGo.transform.position = BlinkLaunchFxAnchor.transform.position;
            _blinkLaunchGo.transform.rotation = Quaternion.LookRotation(-transform.forward);
            _blinkLaunchPs.Play();

            _blinkLandingGo.transform.position = BlinkLandingFxAnchor.transform.position;
            _blinkLandingGo.transform.rotation = Quaternion.LookRotation(-transform.forward);
            _blinkLandingPs.Play();

            var startPosition = LineRendererAnchor.position;

            MakeCloneOnce();

            transform.Translate(transform.forward * BlinkOffset, Space.World);
            SetTrailRendererPositions(startPosition, LineRendererAnchor.position);

            PlaySound();

            _blinkTime = Time.time;
            _isRunning = true;

            AddMaterialAdderAndRun();
            AddShaderAnimatorAndRun();
        }

        private void AddMaterialAdderAndRun()
        {
            if (!AddMaterial)
                return;

            if (_materialAdder != null)
            {
                _materialAdder.Run();
                return;
            }

            _materialAdder = gameObject.AddComponent<IFX_MaterialAdder>();
            _materialAdder.LifeTime = MaterialLifeTime;
            _materialAdder.Targets = MaterialTargets;
            _materialAdder.Material = Material;
            _materialAdder.Setup();
            _materialAdder.Run();
        }

        private void AddShaderAnimatorAndRun()
        {
            if (!AddShaderAnimator)
                return;

            if (_shaderAnimator != null)
            {
                _shaderAnimator.Run();
                return;
            }

            _shaderAnimator = gameObject.AddComponent<IFX_ShaderAnimator>();
            _shaderAnimator.DynamicShaderParameters = new[] { DynamicShaderParameter };
            _shaderAnimator.Run();
        }

        private void MakeCloneOnce()
        {
            if (!MakeClone)
                return;

            var target = CloneSpecificTarget != null ? CloneSpecificTarget.gameObject : gameObject;

            IFX_Cloner.MakeClone(target, CloneLifeTime, CloneMaterial, CloneOpacityAnimation, CloneComponentsWithRequirements);
        }

        private void PlaySound()
        {
            if (AudioSource != null && BlinkAudioClip != null)
                AudioSource.PlayOneShot(BlinkAudioClip);
        }

        private void Awake()
        {
            _points = new Vector3[LineRendererPositionCount];

            _blinkLaunchGo = Instantiate(BlinkLaunchFx);
            _blinkLaunchPs = _blinkLaunchGo.GetComponent<ParticleSystem>();

            _blinkLandingGo = Instantiate(BlinkLandingFx);
            _blinkLandingPs = _blinkLandingGo.GetComponent<ParticleSystem>();

            _lr = Instantiate(LineRenderer);
            _lr.positionCount = LineRendererPositionCount;

            if (LineRendererAnchor == null)
                LineRendererAnchor = transform;
        }

        private void Update()
        {
            if (!_isRunning)
                return;

            if (_lr == null)
                return;

            var lrSize = LineRendererWidthAnimation.Evaluate(Time.time - _blinkTime);
            _lr.startWidth = lrSize;
            _lr.endWidth = lrSize;

            if (lrSize <= 0)
            {
                _isRunning = false;
            }
        }

        private void SetTrailRendererPositions(Vector3 blinkStartPosition, Vector3 blinkEndPosition)
        {
            _pointsCount = LineRendererPositionCount;

            _points[0] = blinkStartPosition;
            _points[_pointsCount - 1] = blinkEndPosition;

            var positionStep = Vector3.Distance(blinkStartPosition, blinkEndPosition) / (_pointsCount - 1);

            var dir = (blinkEndPosition - blinkStartPosition).normalized;

            var offsetPos = blinkStartPosition;

            for (int i = 1; i < _pointsCount - 1; i++)
            {
                offsetPos += dir * positionStep;
                _points[i] = offsetPos;
            }

            _lr.positionCount = _pointsCount;
            _lr.SetPositions(_points);

            _lr.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (_blinkLaunchGo != null)
                Destroy(_blinkLaunchGo.gameObject);
            if (_blinkLandingGo != null)
                Destroy(_blinkLandingGo.gameObject);
            if (_lr != null)
                Destroy(_lr.gameObject);
        }
    }
}