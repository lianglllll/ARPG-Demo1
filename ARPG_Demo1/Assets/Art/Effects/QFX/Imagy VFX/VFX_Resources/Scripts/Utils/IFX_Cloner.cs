using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_Cloner
    {
        public static void MakeClone(GameObject target, float CloneLifeTime, Material CloneMaterial, IFX_AnimationModule CloneOpacityAnimation, Component[] componentsWithRequirements)
        {
            var targetPosition = target.transform.position;

            var clone = Object.Instantiate(target, targetPosition, target.transform.rotation);

            var idxInOrder = 0;
            bool needToUseOrder = componentsWithRequirements != null && componentsWithRequirements.Length > 0;

            foreach (var comp in clone.GetComponentsInChildren<Component>())
            {
                if (!needToUseOrder || comp.GetType() != componentsWithRequirements[idxInOrder].GetType())
                    continue;

                Object.Destroy(comp);
                idxInOrder++;

                if (idxInOrder >= componentsWithRequirements.Length)
                    needToUseOrder = false;
            }

            foreach (var comp in clone.GetComponentsInChildren<Component>())
            {
                if (!(comp is Renderer) && !(comp is Transform) && !(comp is MeshFilter))
                    Object.Destroy(comp);
            }

            var cloneDestroyer = clone.AddComponent<IFX_SelfDestroyer>();
            cloneDestroyer.LifeTime = CloneLifeTime;
            cloneDestroyer.Run();

            IFX_MaterialUtil.RemoveAllMaterials(clone);
            IFX_MaterialUtil.AddMaterial(clone, CloneMaterial);

            var opacityAnimator = clone.AddComponent<IFX_ShaderAnimator>();
            opacityAnimator.DynamicShaderParameters = new[]
            {
                new IFX_DynamicShaderParameter
                {
                    AnimationModule = CloneOpacityAnimation,
                    ParameterName = "_TintColor",
                    Type = IFX_DynamicShaderParameter.ParameterType.ColorAlpha
                }
            };
            opacityAnimator.Setup();
            opacityAnimator.RunWithDelay();
        }
    }
}
