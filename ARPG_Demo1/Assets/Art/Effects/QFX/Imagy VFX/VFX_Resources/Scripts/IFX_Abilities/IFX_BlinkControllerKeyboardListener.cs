using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_BlinkControllerKeyboardListener : MonoBehaviour
    {
        public int MouseButtonCode;

        private IFX_BlinkController _blinkController;

        private void Awake()
        {
            _blinkController = GetComponent<IFX_BlinkController>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(MouseButtonCode))
                _blinkController.Blink();
        }
    }
}
