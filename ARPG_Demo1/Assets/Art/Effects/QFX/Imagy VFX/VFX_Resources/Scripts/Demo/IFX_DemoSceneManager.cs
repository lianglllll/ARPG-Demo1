#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_DemoSceneManager : MonoBehaviour
    {
#if UNITY_EDITOR
        public SceneReferences SceneRef;

        public void LoadScene(string sceneName)
        {
            var scenePath = SceneRef.GetScenePath(sceneName);
            if (scenePath != string.Empty)
            {
                Debug.Log($"found scene: {scenePath}");
                EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));
            }
        }
#endif
    }
}