using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_MaterialReplacer : IFX_ControlledObject
    {
        public Material Material;
        public float LifeTime;

        private Dictionary<Renderer, Material[]> _originalMaterials;

        public override void Setup()
        {
            base.Setup();
            
            _originalMaterials = IFX_MaterialUtil.GetOriginalMaterials(gameObject);
            IFX_MaterialUtil.ReplaceMaterial(gameObject, Material);
        }

        public override void Run()
        {
            base.Run();
            
            //Revert
            IFX_InvokeUtil.RunLater(this, delegate
            {
                IFX_MaterialUtil.ReplaceMaterial(_originalMaterials);
                _originalMaterials.Clear();
            }, LifeTime);
        }
    }
}