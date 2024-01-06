using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_CameraShake : MonoBehaviour
    {
        private Vector3 _originalPos;

        public void Shake(float duration, float amount)
        {
            _originalPos = gameObject.transform.localPosition;
            StartCoroutine(ShakeCoroutine(duration, amount));
        }

        private IEnumerator ShakeCoroutine(float duration, float amount)
        {
            while (duration > 0)
            {
                transform.localPosition = _originalPos + Random.insideUnitSphere * amount;
                duration -= Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _originalPos;

            Destroy(this);
        }
    }
}
