using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
	public class IFX_Minigun : MonoBehaviour
	{
		public ParticleSystem ProjectilePs;
		public ParticleSystem MuzzleFlashPs;
		public ParticleSystem SleevesPs;

		public float FireRate;

		public AudioClip AudioClip;
		public AudioSource AudioSource;

		private bool _isButtonHold;
		private float _time;

		private void LateUpdate()
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (!Physics.Raycast(ray, out hit))
				return;

			var lookDelta = hit.point - transform.position;
			var targetRot = Quaternion.LookRotation(lookDelta);
			transform.rotation = targetRot;

			if (Input.GetMouseButtonDown(0))
				_isButtonHold = true;
			else if (Input.GetMouseButtonUp(0))
				_isButtonHold = false;

			_time += Time.deltaTime;

			if (!_isButtonHold)
				return;

			if (_time < FireRate)
				return;

			ProjectilePs.Emit(1);
			MuzzleFlashPs.Play(true);
			SleevesPs.Emit(1);

			if (AudioSource != null && AudioClip != null)
				AudioSource.PlayOneShot(AudioClip);

			_time = 0;
		}
	}
}
