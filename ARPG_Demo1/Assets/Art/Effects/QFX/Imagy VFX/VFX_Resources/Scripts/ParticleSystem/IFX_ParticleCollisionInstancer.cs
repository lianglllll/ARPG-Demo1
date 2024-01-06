using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class IFX_ParticleCollisionInstancer : MonoBehaviour
    {
        public ParticleSystem CollisionFx;
        public float LifeTime = 1.5f;
        public Vector3 RotationOffset;

        private ParticleSystem _mainPs;
        private readonly List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

        private void Awake()
        {
            _mainPs = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            int collisionEventsCount = _mainPs.GetCollisionEvents(other, _collisionEvents);

            if (collisionEventsCount <= 0)
                return;

            var collisionFx = Instantiate(CollisionFx, _collisionEvents[0].intersection, Quaternion.identity);

            collisionFx.transform.LookAt(_collisionEvents[0].intersection + _collisionEvents[0].normal);
            collisionFx.transform.rotation *= Quaternion.Euler(RotationOffset);

            Destroy(collisionFx.gameObject, LifeTime);
        }
    }
}
