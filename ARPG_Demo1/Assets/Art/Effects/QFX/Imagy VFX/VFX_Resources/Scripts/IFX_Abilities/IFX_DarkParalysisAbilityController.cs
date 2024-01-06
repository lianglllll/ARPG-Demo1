using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_DarkParalysisAbilityController : MonoBehaviour, IFX_IAbilityFx
    {
        public IFX_ObjectFinder ObjectFinder;
        public float AuraClonerLifeTime;
        public IFX_AuraCloner AuraCloner;
        public IFX_AnimatorAffector AnimatorAffector;

        public GameObject DarkParalysisStaff;
        public float InstantiateDelay;
        public Transform[] DarkParalysisStaffsPosition;

        public void Launch()
        {
            var objectsInArea = ObjectFinder.FindObjects(transform.position);

            foreach (var enemy in objectsInArea)
            {
                if (enemy.GetComponent<IFX_AuraCloner>() != null)
                    continue;

                if (AnimatorAffector != null)
                    AnimatorAffector.PlayClip(enemy.transform);

                var targetComponent = IFX_ComponentUtil.CopyComponent(AuraCloner, enemy.transform.root.gameObject);

                targetComponent.Setup();
                targetComponent.RunWithDelay();

                Destroy(targetComponent, AuraClonerLifeTime);
            }

            StartCoroutine("InstantiateStaffs");
        }

        private IEnumerator InstantiateStaffs()
        {
            foreach (var staffPosition in DarkParalysisStaffsPosition)
            {
                Instantiate(DarkParalysisStaff, staffPosition.position, staffPosition.rotation, transform);
                yield return new WaitForSeconds(InstantiateDelay);
            }
        }
    }
}
