using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_AbilityUiManager : MonoBehaviour
    {
        public int AbilityUiMouseButton;
        public int LaunchAbilityMouseButton;

        public GameObject AbilityUi;
        public Vector3 AbilityUiOffset;
        public LayerMask AbilityUiLayerMask;

        public SelectionMode SelectPositionMode;
        public bool LookAtAbility;
        public Vector3 LookAtAddRotation;

        public IFX_AbilityFxManager AbilityFxManager;
        public IFX_AnimatorAbilityFxManager AnimatorAbilityFxManager;
        public string LaunchAbilityName;

        //if SpellPosition is DetectTarget
        public IFX_ObjectFinder ObjectFinder;

        private GameObject _abilityAreaUi;
        private bool _isAbilityAreaButtonHold;

        public Vector3 SelectedPosition { get; private set; }
        public List<Collider> TargetObjects { get; private set; }
        public bool IsPositionSelected { get; private set; }

        private void Awake()
        {
            if (AbilityUi == null)
                return;

            _abilityAreaUi = Instantiate(AbilityUi);
            _abilityAreaUi.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(AbilityUiMouseButton))
                ChangeAbilityAreaVisibility(true);
            else if (Input.GetMouseButtonUp(AbilityUiMouseButton))
                ChangeAbilityAreaVisibility(false);

            if (_isAbilityAreaButtonHold && Input.GetMouseButtonDown(LaunchAbilityMouseButton))
                ActivateAbility();

            if (_isAbilityAreaButtonHold)
                UpdateAbilityAreaUiPosition();
        }

        private void ChangeAbilityAreaVisibility(bool isVisible)
        {
            if (_abilityAreaUi == null)
                return;

            UpdateAbilityAreaUiPosition();
            _abilityAreaUi.SetActive(isVisible);
            _isAbilityAreaButtonHold = isVisible;
        }

        private void UpdateAbilityAreaUiPosition()
        {
            if (_abilityAreaUi == null)
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 500, AbilityUiLayerMask))
                return;

            _abilityAreaUi.transform.position = hit.point + AbilityUiOffset;
        }

        private void ActivateAbility()
        {
            LaunchAbility();

            if (!IsPositionSelected)
                return;

            if (LookAtAbility)
            {
                var lookAtPos = new Vector3(SelectedPosition.x, transform.position.y, SelectedPosition.z);
                lookAtPos += LookAtAddRotation;
                transform.LookAt(lookAtPos);
            }

            if (AbilityFxManager != null)
            {
                Vector3 launchPosition;

                switch (SelectPositionMode)
                {
                    case SelectionMode.Position:
                        launchPosition = SelectedPosition;
                        break;
                    case SelectionMode.DetectObject:
                        launchPosition = TargetObjects.First().transform.position;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AbilityFxManager.LaunchFx(LaunchAbilityName, launchPosition);
            }

            if (AnimatorAbilityFxManager != null)
                AnimatorAbilityFxManager.PlayAbilityAnimation(LaunchAbilityName);
        }

        private void LaunchAbility()
        {
            TargetObjects = null;
            SelectedPosition = Vector3.zero;
            IsPositionSelected = false;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 500, AbilityUiLayerMask))
                return;

            Vector3 selectedPosition;

            switch (SelectPositionMode)
            {
                case SelectionMode.Position:
                    selectedPosition = hit.point;
                    IsPositionSelected = true;
                    break;
                case SelectionMode.DetectObject:
                    var foundObjects = ObjectFinder.FindObjects(hit.point);
                    if (foundObjects.Count == 0)
                        return;

                    selectedPosition = foundObjects.First().transform.position;
                    TargetObjects = foundObjects;

                    IsPositionSelected = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SelectedPosition = selectedPosition;
        }

        public enum SelectionMode
        {
            Position,
            DetectObject
        }
    }
}