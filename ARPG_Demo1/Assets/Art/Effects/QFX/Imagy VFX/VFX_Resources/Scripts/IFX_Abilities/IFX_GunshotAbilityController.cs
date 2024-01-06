using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_GunshotAbilityController : MonoBehaviour, IFX_IActivableAnimatorAbility
    {
        public string LaunchAnchorName;
        public IFX_ShaderAnimator ShaderAnimator;
        public IFX_ManualProjectileWeapon ProjectileWeapon;

        private readonly IFX_TargetAttacher _ifxTargetAttacher = new IFX_TargetAttacher();

        public Transform Emitter { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }

        public void Launch()
        {
            ShaderAnimator.Run();

            _ifxTargetAttacher.FindAndAttach(LaunchAnchorName, Emitter, transform);

            ProjectileWeapon.EarlyAim();
        }

        public void Activate()
        {
            ProjectileWeapon.Shoot();
        }

        private void Update()
        {
            _ifxTargetAttacher.Update();
        }
    }
}
