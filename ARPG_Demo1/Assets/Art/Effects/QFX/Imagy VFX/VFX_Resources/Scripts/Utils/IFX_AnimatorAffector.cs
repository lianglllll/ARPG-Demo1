using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [Serializable]
    public class IFX_AnimatorAffector
    {
        public bool IsEnabled;
        public string ClipName;

        public void PlayClip(Transform target)
        {
            if (IsEnabled)
            {
                var animator = target.root.GetComponent<Animator>();
                if (animator != null)
                    animator.Play(ClipName);
            }
        }
    }
}
