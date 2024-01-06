using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
	[RequireComponent(typeof(Animator))]
	public class IFX_AnimatorAbilityFxManager : MonoBehaviour
	{
		public IFX_AnimatorAbilityFx[] AnimatorAbilityFxs;

		private readonly Dictionary<string, GameObject> _spellNameToGameObjectMap = new Dictionary<string, GameObject>();

		private IFX_AbilityUiManager _ifxAbilityUiManager;

		private Animator _animator;

		public void PlayAbilityAnimation(string abilityName)
		{
			var abilityFx = AnimatorAbilityFxs
				.SingleOrDefault(t => t.FxName == abilityName);

			if (abilityFx == default(IFX_AnimatorAbilityFx))
				return;

			_animator.Play(abilityFx.LaunchStateName);
		}

		//Will be called from animator
		public void LaunchFx(string abilityName)
		{
			var abilityFx = AnimatorAbilityFxs
				.SingleOrDefault(t => t.FxName == abilityName);

			if (abilityFx == default(IFX_AnimatorAbilityFx))
				return;

			var abilityGo = Instantiate(abilityFx.Fx);
			abilityGo.transform.position = transform.position;
			abilityGo.transform.rotation = transform.rotation;

			if (abilityFx.AttachAnchor)
			{
				var anchor = IFX_TargetAttacher.FindChildByRecursion(transform.root, abilityFx.AnchorName);
				if (anchor != null)
				{
					abilityGo.transform.rotation = Quaternion.Euler(abilityFx.RotationOffset);
					abilityGo.transform.position = anchor.position;
					if (abilityFx.UseAnchorRotation)
						abilityGo.transform.rotation = anchor.rotation;
				}
			}

			var spellPs = abilityGo.GetComponent<ParticleSystem>();
			if (spellPs != null)
				spellPs.Play(true);

			if (abilityFx.IsActivationRequired)
				_spellNameToGameObjectMap[abilityName] = abilityGo;

			var animatorAbility = abilityGo.GetComponent<IFX_IAnimatorAbility>();
			if (animatorAbility != null)
			{
				animatorAbility.Emitter = transform;
				animatorAbility.TargetPosition = _ifxAbilityUiManager.SelectedPosition;

				var targets = _ifxAbilityUiManager.TargetObjects;
				if (targets != null && targets.Any())
					animatorAbility.Target = targets.First().transform;

				animatorAbility.Launch();
			}

			//if (abilityFx.LaunchAudioClip != null)
			//{
			//	var abilityAudioSource = abilityGo.GetComponent<AudioSource>();
			//	if (abilityAudioSource != null)
			//		abilityAudioSource.PlayOneShot(abilityFx.LaunchAudioClip);
			//}
		}

		//Will be called from animator
		public void ActivateFx(string abilityName)
		{
			var abilityFx = AnimatorAbilityFxs
				.SingleOrDefault(t => t.FxName == abilityName);

			if (abilityFx == default(IFX_AnimatorAbilityFx))
				return;

			var abilityGo = _spellNameToGameObjectMap[abilityName];

			if (abilityGo == null)
				return;

			_spellNameToGameObjectMap.Remove(abilityName);

			if (_ifxAbilityUiManager != null && !_ifxAbilityUiManager.IsPositionSelected)
				return;

			//if (abilityFx.ActivateAudioClip != null)
			//{
			//	var abilityAudioSource = abilityGo.GetComponent<AudioSource>();
			//	if (abilityAudioSource != null)
			//		abilityAudioSource.PlayOneShot(abilityFx.ActivateAudioClip);
			//}

			var animSpell = abilityGo.GetComponent<IFX_IActivableAnimatorAbility>();
			if (animSpell != null)
				animSpell.Activate();
		}

		private void Awake()
		{
			_ifxAbilityUiManager = GetComponent<IFX_AbilityUiManager>();
			_animator = GetComponent<Animator>();
		}
	}
}