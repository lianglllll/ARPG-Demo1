using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_ShaderAnimator : IFX_ControlledObject
    {
        public IFX_DynamicShaderParameter[] DynamicShaderParameters;
        public Transform Target;
        public bool AffectChildren = true;

        private float _startTime;
        private List<Material> _materials;

        public override void Setup()
        {
            if (Target == null)
                Target = transform;

            base.Setup();

            _startTime = Time.time;
            _materials = new List<Material>();

            var rends = AffectChildren
                ? Target.GetComponentsInChildren<Renderer>(true)
                : Target.GetComponents<Renderer>();

            foreach (var rend in rends)
                _materials.AddRange(rend.materials);

            UpdateMaterials(0);
        }

        public override void Run()
        {
            base.Run();
            _startTime = Time.time;
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            var time = Time.time - _startTime;

            UpdateMaterials(time);
        }

        private void UpdateMaterials(float time)
        {
            if (DynamicShaderParameters == null)
                return;

            foreach (var shaderParameter in DynamicShaderParameters)
            {
                if (shaderParameter.AnimationModule == null)
                    continue;

                var val = shaderParameter.AnimationModule.Evaluate(time);

                foreach (var material in _materials)
                {
                    if (!material.HasProperty(shaderParameter.ParameterName))
                        continue;

                    if (shaderParameter.Type == IFX_DynamicShaderParameter.ParameterType.Float)
                    {
                        material.SetFloat(shaderParameter.ParameterName, val);
                    }
                    else if (shaderParameter.Type == IFX_DynamicShaderParameter.ParameterType.ColorAlpha)
                    {
                        var color = material.GetColor(shaderParameter.ParameterName);
                        color.a = val;
                        material.SetColor(shaderParameter.ParameterName, color);
                    }
                }
            }
        }
    }
}