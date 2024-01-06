using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [Serializable]
    public class IFX_TargetAttacher
    {
        private Transform _transform;

        public Vector3 RotationOffset { get; set; }
        public Vector3 Offset { get; set; }
        public Transform Target { get; private set; }
        public bool IsAttached { get; private set; }

        public void Attach(Transform anchor, Transform selfTransform)
        {
            _transform = selfTransform;
            Target = anchor;
            IsAttached = true;
        }

        public void FindAndAttach(string anchorName, Transform parent, Transform selfTransform)
        {
            var anchor = FindChildByRecursion(parent, anchorName);
            if (anchor == null)
            {
                Debug.LogWarning("FxAnchor not found");
                return;
            }

            Target = anchor;
            _transform = selfTransform;

            IsAttached = true;
        }

        public void DeAttach()
        {
            IsAttached = false;
            _transform = null;
        }

        public void Update()
        {
            if (!IsAttached)
                return;

            _transform.position = Target.position + Offset;
            _transform.rotation = Target.rotation * Quaternion.Euler(RotationOffset);
        }

        public static Transform FindChildByRecursion(Transform aParent, string aName)
        {
            if (aParent == null)
                return null;

            var result = aParent.Find(aName);

            if (result != null)
                return result;

            foreach (Transform child in aParent)
            {
                result = FindChildByRecursion(child, aName);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}