using System;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [Serializable]
    public class IFX_AnimationModule
    {
        public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float ValueMultiplier = 1f;
        public float TimeMultiplier = 1f;

        public bool IsAnimationFinished { get; set; }

        public float Evaluate(float time)
        {
            var mTime = time / TimeMultiplier;

            var lastTime = AnimationCurve.keys.Last().time;
            if (mTime > lastTime)
                IsAnimationFinished = true;

            var eval = AnimationCurve.Evaluate(mTime) * ValueMultiplier;
            return eval;
        }
    }
}