using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [Serializable]
    public sealed class IFX_ManualProjectileWeapon : MonoBehaviour
    {
        public GameObject LaunchPs;
        public GameObject ImpactPs;
        public Transform LaunchTransform;
        public IFX_LightAnimator LightAnimator;
        public GameObject Projectile;
        public float ProjectileSpeed;

        public bool IsEarlyAimingEnabled;

        public AudioSource AudioSource;
        public AudioClip ShotAudioClip;

        private ParticleSystem _launchPs;

        public void EarlyAim()
        {
            if (!IsEarlyAimingEnabled)
                return;

            Aim();
        }

        public void Shoot()
        {
            if (!IsEarlyAimingEnabled)
                Aim();

            Fire();
        }

        private void Aim()
        {
            Vector3 position;
            Quaternion rotation;

            if (LaunchTransform != null)
            {
                position = LaunchTransform.position;
                rotation = LaunchTransform.rotation;
            }
            else
            {
                position = transform.position;
                rotation = transform.rotation;
            }

            _launchPs.transform.position = position;
            _launchPs.transform.rotation = rotation;
            _launchPs.Play(true);
        }

        private void Fire()
        {
            Vector3 position;
            Quaternion rotation;

            if (LaunchTransform != null)
            {
                position = LaunchTransform.position;
                rotation = LaunchTransform.rotation;
            }
            else
            {
                position = transform.position;
                rotation = transform.rotation;
            }

            var goProjectile = Instantiate(Projectile, position, rotation);
            var projectile = goProjectile.GetComponent<IFX_SimpleProjectile>();
            if (projectile != null)
            {
                projectile.Speed = ProjectileSpeed;
                projectile.ImpactPs = ImpactPs;
            }

            if (LightAnimator != null)
                LightAnimator.Run();

            if (AudioSource != null && ShotAudioClip != null)
                AudioSource.PlayOneShot(ShotAudioClip);
        }

        private void Awake()
        {
            var launchGo = Instantiate(LaunchPs);
            var originalScale = launchGo.transform.localScale;
            launchGo.transform.parent = transform;
            launchGo.transform.localScale = originalScale;
            launchGo.transform.rotation = LaunchTransform.rotation;
            _launchPs = launchGo.GetComponent<ParticleSystem>();
        }
    }
}
