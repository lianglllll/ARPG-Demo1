using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_GroundAttacher : IFX_ControlledObject
    {
        public LayerMask LayerMask;
        public float MaxDistance = 100;

        public bool OverrideRotation;

        public Vector3 Offset;

        private bool _isAttached;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void LateUpdate()
        {
            if (!IsRunning || _isAttached)
                return;

            RaycastHit raycastHit;
            var hitPos = _transform.position.y;

            if (Physics.Raycast(_transform.position, Vector3.down, out raycastHit, MaxDistance, LayerMask))
                hitPos = Mathf.Min(hitPos, raycastHit.point.y);

            if (Physics.Raycast(_transform.position, Vector3.up, out raycastHit, MaxDistance, LayerMask))
                hitPos = Mathf.Min(hitPos, raycastHit.point.y);

            if (Physics.Raycast(_transform.position, Vector3.left, out raycastHit, MaxDistance, LayerMask))
                hitPos = Mathf.Min(hitPos, raycastHit.point.y);

            if (Physics.Raycast(_transform.position, Vector3.right, out raycastHit, MaxDistance, LayerMask))
                hitPos = Mathf.Min(hitPos, raycastHit.point.y);

            if (!(Mathf.Abs(_transform.position.y - hitPos) > 0.01f))
                return;

            _isAttached = true;

            transform.position = new Vector3(_transform.position.x + Offset.x, hitPos + Offset.y, _transform.position.z + Offset.z);

            if (OverrideRotation)
                transform.rotation = Quaternion.LookRotation(Vector3.forward);

            Stop();
        }
    }
}