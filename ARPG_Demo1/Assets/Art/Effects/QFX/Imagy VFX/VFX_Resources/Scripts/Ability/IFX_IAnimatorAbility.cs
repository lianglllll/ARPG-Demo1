using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public interface IFX_IAnimatorAbility
    {
        Transform Emitter { get; set; }
        Vector3 TargetPosition { get; set; }
        Transform Target { get; set; }
        void Launch();
    }
}