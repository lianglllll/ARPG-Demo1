using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_ColliderIntersectionFxInstancer : MonoBehaviour
    {
        public GameObject Fx;

        public IFX_ObjectFinder ObjectFinder;

        private readonly List<Collider> _foundObjects = new List<Collider>();

        private void LateUpdate()
        {
            var targets = ObjectFinder.FindObjects(transform.position);
            if (!_foundObjects.Any() && !targets.Any())
                return;

            foreach (var target in targets)
            {
                if (_foundObjects.All(t => t.name != target.name))
                {
                    Instantiate(Fx, transform.position, Quaternion.identity);
                    _foundObjects.Add(target);
                }
            }

            var objectsToRemove = _foundObjects.Except(targets).ToList();
            foreach (var toRemove in objectsToRemove)
                _foundObjects.Remove(toRemove);
        }
    }
}
