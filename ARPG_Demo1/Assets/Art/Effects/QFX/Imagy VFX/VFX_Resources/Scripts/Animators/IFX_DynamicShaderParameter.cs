using System;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [Serializable]
    public class IFX_DynamicShaderParameter
    {
        public IFX_AnimationModule AnimationModule;
        public string ParameterName;
        public ParameterType Type;

        public enum ParameterType
        {
            Float,
            ColorAlpha
        }
    }
}