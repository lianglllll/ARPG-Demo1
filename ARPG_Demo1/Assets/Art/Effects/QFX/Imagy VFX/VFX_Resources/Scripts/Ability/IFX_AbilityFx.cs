using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [CreateAssetMenu(fileName = "AbilityFx", menuName = "QFX/IFX/AbilityFx", order = 1)]
    public class IFX_AbilityFx : ScriptableObject
    {
        public string FxName;
        public GameObject Fx;
        public Vector3 Offset;
        public AudioClip LaunchAudioClip;
    }
}
