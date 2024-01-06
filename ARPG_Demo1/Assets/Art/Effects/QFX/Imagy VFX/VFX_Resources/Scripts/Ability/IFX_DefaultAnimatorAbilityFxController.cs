using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_DefaultAnimatorAbilityFxController : MonoBehaviour, IFX_IAnimatorAbility
    {
        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

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
            transform.position = ResultTargetPosition;
        }
    }
}
