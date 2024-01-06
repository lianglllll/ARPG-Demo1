using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_MaterialAdder : IFX_ControlledObject
    {
        public Material Material;
        public float LifeTime;

        public GameObject[] Targets;

        private readonly Dictionary<Renderer, Material[]> _rendToMaterialsMap = new Dictionary<Renderer, Material[]>();

        private bool _isMaterialAdded;

        private float _addedTime;

        public override void Run()
        {
            base.Run();

            _addedTime = Time.time;

            if (_isMaterialAdded)
                return;

            if (Targets != null && Targets.Length > 0)
            {
                foreach (var target in Targets)
                {
                    GetMaterialsAndFillCollection(target);
                }
            }
            else
            {
                GetMaterialsAndFillCollection(gameObject);
            }

            _isMaterialAdded = true;


            IFX_InvokeUtil.RunLater(this, Stop, LifeTime);
        }

        public override void Stop()
        {
            base.Stop();

            var timeDiff = Time.time - _addedTime;
            if (timeDiff < LifeTime)
            {
                //call again
                IFX_InvokeUtil.RunLater(this, Stop, LifeTime - timeDiff);
                return;
            }

            IFX_MaterialUtil.ReplaceMaterial(_rendToMaterialsMap);
            _rendToMaterialsMap.Clear();
            _isMaterialAdded = false;
        }

        private void GetMaterialsAndFillCollection(GameObject targetGo)
        {
            var rendToMaterials = IFX_MaterialUtil.GetOriginalMaterials(targetGo);
            foreach (var rToMat in rendToMaterials)
                _rendToMaterialsMap[rToMat.Key] = rToMat.Value;
            IFX_MaterialUtil.AddMaterial(targetGo, Material);
        }
    }
}