using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
	public class IFX_TimelineFxInstancer : MonoBehaviour
	{
		public Transform FxAnchor;
		public GameObject Fx;

		public bool UseAnchorRotation;

		private bool _isEnabled;

		private void OnEnable()
		{
			if (_isEnabled)
				return;

			var fx = Instantiate(Fx);
			fx.transform.position = FxAnchor.transform.position;

			if (UseAnchorRotation)
				fx.transform.rotation = FxAnchor.transform.rotation;

			var ps = fx.GetComponent<ParticleSystem>();
			if (ps != null)
				ps.Play(true);

			_isEnabled = true;
		}

		private void OnDisable()
		{
			_isEnabled = false;
		}
	}
}
