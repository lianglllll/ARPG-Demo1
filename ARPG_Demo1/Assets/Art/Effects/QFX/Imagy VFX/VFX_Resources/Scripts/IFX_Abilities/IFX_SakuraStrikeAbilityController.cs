using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_SakuraStrikeAbilityController : MonoBehaviour, IFX_IAnimatorAbility
    {
        public ParticleSystem MuzzleFlashPs;
        public ParticleSystem ImpactPs;
        public LineRenderer StrikeLineRenderer;

        public int StrikeCount;
        public float StrikeDelay;
        public float StrikeRadius;
        public Vector3 StrikeOffset;

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

        private Vector3 ResultTargetPosition
        {
            get
            {
                if (Target == null)
                    return TargetPosition;
                return Target.transform.position;
            }
        }

        private IEnumerator InstantiateStrikes()
        {
            var resultTargetPosition = ResultTargetPosition;

            for (int i = 0; i < StrikeCount; i++)
            {
                var pos = resultTargetPosition + StrikeOffset;
                var randomPos = Random.insideUnitCircle * StrikeRadius;
                pos += new Vector3(randomPos.x, pos.y, randomPos.y);

                var strike = Instantiate(MuzzleFlashPs, pos, Quaternion.identity);

                var relativePos = (resultTargetPosition - strike.transform.position).normalized;
                var rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                strike.transform.rotation = rotation;

                var lr = Instantiate(StrikeLineRenderer);
                lr.SetPosition(0, pos);
                lr.SetPosition(1, resultTargetPosition);

                Instantiate(ImpactPs, resultTargetPosition, Quaternion.identity);

                yield return new WaitForSeconds(StrikeDelay);
            }
        }

        public void Launch()
        {
            StartCoroutine("InstantiateStrikes");
        }
    }
}
