using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    [RequireComponent(typeof(IFX_AbilityUiManager))]
    public class IFX_AbilityFxManager : MonoBehaviour
    {
        public IFX_AbilityFx[] AbilityFxs;

        public void LaunchFx(string abilityName, Vector3 launchPosition)
        {
            var abilityFx = AbilityFxs.SingleOrDefault(t => t.FxName == abilityName);

            if (abilityFx == default(IFX_AbilityFx))
                return;

            var abilityGo = Instantiate(abilityFx.Fx);
            abilityGo.transform.position = launchPosition + abilityFx.Offset;

            if (abilityFx.LaunchAudioClip != null)
            {
                var abilityAudioSource = abilityGo.GetComponent<AudioSource>();
                if (abilityAudioSource != null)
                    abilityAudioSource.PlayOneShot(abilityFx.LaunchAudioClip);
            }

            var ability = abilityGo.GetComponentInChildren<IFX_IAbilityFx>();
            if (ability != null)
                ability.Launch();
        }
    }
}