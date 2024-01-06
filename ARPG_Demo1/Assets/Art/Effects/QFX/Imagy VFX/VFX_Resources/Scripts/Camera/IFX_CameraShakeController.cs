using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_CameraShakeController : MonoBehaviour
    {
        public float Amount;
        public float Duration;

        private void OnEnable()
        {
            var mainCamera = Camera.main;
            var cameraShake = mainCamera.gameObject.AddComponent<IFX_CameraShake>();
            cameraShake.Shake(Duration, Amount);
        }
    }
}
