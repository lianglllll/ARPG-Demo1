using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{ 
    public class IFX_AudioClipPlayer : IFX_ControlledObject
    {
        public AudioSource AudioSource;
        public AudioClip AudioClip;

        public override void Run()
        {
            if (AudioSource != null && AudioClip != null)
                AudioSource.PlayOneShot(AudioClip);
        }
    }
}
