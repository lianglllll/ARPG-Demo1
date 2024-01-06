// ReSharper disable once CheckNamespace
using System.Collections.Generic;
using UnityEngine;

namespace QFX.IFX
{
	[ExecuteInEditMode]
	public class IFX_MaterialSetter : MonoBehaviour
	{
		public int RenderQueue;

		//private List<Material> _materials = new List<Material>();

		private void Awake()
		{
			var renderers = GetComponentsInChildren<Renderer>();
			foreach (var rend in renderers)
				rend.sharedMaterial.renderQueue = RenderQueue;
				//_materials.Add(rend.sharedMaterial);

			//foreach (var material in _materials)
			//	material.renderQueue = RenderQueue;
		}

		private void LateUpdate()
		{
			
		}
	}
}
