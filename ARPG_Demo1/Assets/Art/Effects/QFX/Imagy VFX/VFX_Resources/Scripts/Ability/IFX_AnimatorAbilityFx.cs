using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [CreateAssetMenu(fileName = "AnimatorAbilityFx", menuName = "QFX/IFX/AnimatorAbilityFx", order = 1)]
    public class IFX_AnimatorAbilityFx : ScriptableObject
    {
        public string FxName;
        public GameObject Fx;
        public string LaunchStateName;
        public bool IsActivationRequired;
        //public AudioClip LaunchAudioClip;
        //public AudioClip ActivateAudioClip;
        public bool AttachAnchor;
        public string AnchorName;
        public bool UseAnchorRotation;
        public Vector3 RotationOffset;
    }
}