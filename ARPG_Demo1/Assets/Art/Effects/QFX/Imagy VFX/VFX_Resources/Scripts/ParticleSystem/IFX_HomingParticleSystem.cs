using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ParticleSystem))]
    public class IFX_HomingParticleSystem : IFX_ControlledObject
    {
        public Transform Target;
        public float Force;

        private ParticleSystem _ps;
        private ParticleSystem.MainModule _psMain;
        private ParticleSystem.Particle[] _particles;

        public Action TargetReached;

        private void LateUpdate()
        {
            if (!IsRunning)
                return;

            if (Target == null)
                return;

            _ps.GetParticles(_particles);

            var originalTargetPos = Target.position;

            //if (FollowColliderCenter)
            //{
            //    var coll = Target.GetComponent<Collider>();
            //    if (coll != null)
            //        originalTargetPos = coll.bounds.center;
            //}
            //else originalTargetPos = Target.position;

            Vector3 resultTargetPos;

            switch (_psMain.simulationSpace)
            {
                case ParticleSystemSimulationSpace.Local:
                    {
                        resultTargetPos = transform.InverseTransformPoint(originalTargetPos);
                        break;
                    }
                case ParticleSystemSimulationSpace.Custom:
                    {
                        resultTargetPos = _psMain.customSimulationSpace.InverseTransformPoint(originalTargetPos);
                        break;
                    }
                case ParticleSystemSimulationSpace.World:
                    {
                        resultTargetPos = originalTargetPos;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int particleCount = _ps.particleCount;

            for (int i = 0; i < particleCount; i++)
            {
                var dir = Vector3.Normalize(resultTargetPos - _particles[i].position);
                var force = dir * Force;

                _particles[i].velocity += force;
            }

            _ps.SetParticles(_particles, particleCount);
        }

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
            _psMain = _ps.main;
            _particles = new ParticleSystem.Particle[_psMain.maxParticles];
        }
    }
}
