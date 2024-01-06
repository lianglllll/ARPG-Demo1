using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [Serializable]
    public class IFX_ObjectFinder
    {
        public float Radius;
        public LayerMask LayerMask;
        public string Tag;
        public string Name;

        public List<Collider> FindObjects(Vector3 position)
        {
            var objectsInArea = IFX_ObjectAreaFinder.FindObjects(position, Radius, LayerMask, Tag, Name);
            return objectsInArea;
        }
    }
}
