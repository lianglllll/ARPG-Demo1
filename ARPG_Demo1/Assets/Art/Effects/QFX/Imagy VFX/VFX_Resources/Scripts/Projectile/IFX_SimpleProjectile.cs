using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_SimpleProjectile : MonoBehaviour
    {
        public ParticleSystem MovementPS;
        public Collider SelfCollider;

        public bool DestroyAfterCollision;
        public float DestroyAfterCollisionTimeout;

        public bool IsSingleCollisionMode = true;

        public AudioSource AudioSource;

        [HideInInspector]
        public GameObject ImpactPs;
        [HideInInspector]
        public float Speed = 1;
        [HideInInspector]
        public AudioClip ImpactAudioClip;

        private Transform _transform;

        private bool _wasCollided;
        private ParticleSystem _impactPs;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_wasCollided)
                return;

            _transform.position += _transform.forward * Speed * Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_wasCollided && IsSingleCollisionMode)
                return;

            if (!collision.contacts.Any())
                return;

            if (!_wasCollided)
            {
                if (DestroyAfterCollision)
                    Destroy(gameObject, DestroyAfterCollisionTimeout);

                if (IsSingleCollisionMode)
                {
                    SelfCollider.enabled = false;
                }

                var impactGo = Instantiate(ImpactPs);
                var impactPs = impactGo.GetComponent<ParticleSystem>();

                impactPs.transform.rotation = Quaternion.FromToRotation(impactPs.transform.up, collision.contacts[0].normal) *
                                              impactPs.transform.rotation;
                impactPs.transform.position = collision.contacts[0].point;
                impactPs.Play(true);

                if (AudioSource != null && ImpactAudioClip != null)
                    AudioSource.PlayOneShot(ImpactAudioClip);

                MovementPS.Stop();
            }

            _wasCollided = true;
        }
    }
}
