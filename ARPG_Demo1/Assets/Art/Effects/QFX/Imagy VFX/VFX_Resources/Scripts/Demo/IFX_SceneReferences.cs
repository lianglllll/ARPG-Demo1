#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [CreateAssetMenu(menuName = "QFX/IFX/SceneRef")]
    public class SceneReferences : ScriptableObject
    {
#if UNITY_EDITOR
        public SceneAsset[] Scenes;

        public string GetScenePath(string _name)
        {
            foreach (var s in Scenes)
            {
                if (s.name == _name)
                {
                    return AssetDatabase.GetAssetOrScenePath(s);
                }
            }

            Debug.LogError($"Can't find scenePath for {_name}");
            return string.Empty;
        }
#endif
    }
}