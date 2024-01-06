using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_DemoButtonParent : MonoBehaviour
    {
#if UNITY_EDITOR
        public void Awake()
        {
            var button = GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(LoadDemoScene);
            }
            else
            {
                Debug.LogError($"Can't find button: {transform.name}");
            }
        }

        private void LoadDemoScene()
        {
            var demoSceneManager = FindObjectOfType<IFX_DemoSceneManager>();
            demoSceneManager.LoadScene(gameObject.name);
        }
#endif
    }
}