using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_DangerousPumpkinAbilityController : MonoBehaviour, IFX_IAbilityFx
    {
        public ParticleSystem JumpingPs;
        public ParticleSystem LandingPs;
        public ParticleSystem DeathPs;

        public float AccelerationDistance;
        public float AccelerationFactor;
        public Animator Animator;

        public IFX_ObjectFinder ObjectFinder;

        public string DeatchAnimationStateName;

        public Transform Pumpkin;
        public Transform BrokenPumpkin;
        public float BrokenPartForce;

        //public IFX_DynamicShaderParameter DissolveBrokenPartShaderParameter;

        public AudioSource AudioSource;
        public AudioClip JumpingAudioClip;
        public AudioClip DeathAudioClip;

        private Transform _target;

        private ParticleSystem _jumpingPs;
        private ParticleSystem _landingPs;
        private ParticleSystem _deathPs;

        private Transform[] _brokenParts;

        private bool _isPumpkinAccelerated;

        public void Launch()
        {
            _jumpingPs = Instantiate(JumpingPs, parent: transform.root);
            _landingPs = Instantiate(LandingPs, parent: transform.root);
            _deathPs = Instantiate(DeathPs, transform.root);

            _brokenParts = BrokenPumpkin.GetComponentsInChildren<Transform>(true);
        }

        //will be called by Animation (CTRL + 6)
        public void LaunchJumpFx()
        {
            if (_jumpingPs == null)
                return;

            _jumpingPs.transform.position = transform.position;
            _jumpingPs.Play(true);
        }

        //will be called by Animation (CTRL + 6)
        public void LaunchLandFx()
        {
            if (_landingPs == null)
                return;

            _landingPs.transform.position = transform.position;
            _landingPs.Play(true);


            if (AudioSource != null)
                AudioSource.PlayOneShot(JumpingAudioClip);
        }

        //will be called by Animation (CTRL + 6)
        public void LaunchDeatchFx()
        {
            Pumpkin.gameObject.SetActive(false);

            if (_brokenParts == null)
                _brokenParts = BrokenPumpkin.GetComponentsInChildren<Transform>(true);

            foreach (var pumpkinPart in _brokenParts)
            {
                var rigidBody = pumpkinPart.GetComponent<Rigidbody>();

                if (rigidBody == null)
                    continue;

                pumpkinPart.gameObject.SetActive(true);
                var dir = (pumpkinPart.transform.position - Pumpkin.transform.position).normalized * BrokenPartForce;
                rigidBody.AddForce(dir, ForceMode.Impulse);

                //var shaderAnimator = pumpkinPart.gameObject.AddComponent<IFX_ShaderAnimator>();
                //shaderAnimator.DynamicShaderParameters = new IFX_DynamicShaderParameter[1]
                //{
                //    DissolveBrokenPartShaderParameter
                //};
                //shaderAnimator.Run();
            }

            if (_deathPs != null)
            {
                _deathPs.transform.position = transform.position;
                _deathPs.Play();
            }


            if (AudioSource != null)
                AudioSource.PlayOneShot(DeathAudioClip);
        }

        private void Update()
        {
            if (_target != null)
            {
                transform.LookAt(new Vector3(_target.position.x, transform.position.y, _target.position.z));

                var distanceToTarget = Vector3.Distance(transform.position, _target.position);

                if (distanceToTarget < AccelerationDistance)
                {
                    if (!_isPumpkinAccelerated)
                    {
                        Animator.speed *= AccelerationFactor;
                        _isPumpkinAccelerated = true;
                    }

                    if (distanceToTarget <= 0.1f)
                        Animator.Play(DeatchAnimationStateName);
                }

                return;
            }

            var colliders = ObjectFinder.FindObjects(transform.position);

            if (!colliders.Any())
                return;

            _target = colliders.First().transform;
        }
    }
}
