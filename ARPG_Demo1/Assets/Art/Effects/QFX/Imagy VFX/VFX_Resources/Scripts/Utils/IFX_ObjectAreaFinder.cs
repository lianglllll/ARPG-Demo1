using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public static class IFX_ObjectAreaFinder
    {
        public static List<Collider> FindObjects(Vector3 position, float areaRadius, LayerMask layerMask, string tag, string name)
        {
            var colliders = Physics.OverlapSphere(position, areaRadius, layerMask);

            var result = !string.IsNullOrEmpty(tag) ?
                colliders.Where(c => c.transform.root.tag == tag).ToList() :
                colliders.ToList();

            result = !string.IsNullOrEmpty(name) ?
                result.Where(c => c.transform.root.name.ToLowerInvariant().Contains(name.ToLowerInvariant())).ToList() :
                result.ToList();

            return result;
        }
    }
}