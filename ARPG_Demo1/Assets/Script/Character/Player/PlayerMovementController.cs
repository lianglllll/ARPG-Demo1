using GGG.Tool;
using MyARPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.Movement
{
    public class PlayerMovementController : CharacterMovementControllerBase
    {
        private float _ratationAngle;
        private float _angleVelocity = 0f;
        [SerializeField]
        private float _rotationSmoothTime;
        private Transform _mainCamera;

        //�Ų������
        private float _nextFootTime;
        [SerializeField] private float _slowFootTime;
        [SerializeField] private float _fastFootTime;

        //��ɫ��Ŀ�곯��
        private Vector3 _characterTargetDirection;



        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            UpdateAnimation();
            CharacterRotationControl();
        }

        //�����������ת����
        private void CharacterRotationControl()
        {
            if (!_characterIsOnGroud ) return;

            if (_animator.GetBool(AnimationID.HasInputID))
            {
                //��ȡ������ĳ���+��ҿ��������ƶ������ĽǶ���ת
                _ratationAngle = _mainCamera.eulerAngles.y + Mathf.Atan2(InputManager.Instance.Movement.x, InputManager.Instance.Movement.y) * Mathf.Rad2Deg ;
            }

            if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))//����˵�����ʱ��Ͳ�����ת
            {
                //if (_animator.GetFloat(AnimationID.DeltaAngleID) < -135f) return;
                //if (_animator.GetFloat(AnimationID.DeltaAngleID) > 135f) return;

                //����ɫ��yΪ�����ƽ����ת
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _ratationAngle, ref _angleVelocity, _rotationSmoothTime);
                //�õ�����Ҫת����ĸ�����
                _characterTargetDirection = Quaternion.Euler(0f, _ratationAngle, 0f) * Vector3.forward;
            }
            _animator.SetFloat(AnimationID.DeltaAngleID, DevelopmentToos.GetDeltaAngle(transform,_characterTargetDirection.normalized));
        }


        //�����л����߼�
        private void UpdateAnimation()
        {
            if (!_characterIsOnGroud) return;

            _animator.SetBool(AnimationID.HasInputID, InputManager.Instance.Movement != Vector2.zero);

            if (_animator.GetBool(AnimationID.HasInputID)){

                if (InputManager.Instance.Run)
                {
                    _animator.SetBool(AnimationID.RunID, true);
                }
                _animator.SetFloat(AnimationID.MovementID, _animator.GetBool(AnimationID.RunID) ? 2f : InputManager.Instance.Movement.sqrMagnitude, 0.25f, Time.deltaTime);
                
                SetCharacterFootSound();
            }
            else
            {

                _animator.SetFloat(AnimationID.MovementID,0f, 0.25f, Time.deltaTime);
                if (_animator.GetFloat(AnimationID.MovementID) < 0.01f)
                {
                    _animator.SetFloat(AnimationID.MovementID, 0f);
                }

                if (_animator.GetFloat(AnimationID.MovementID) < 0.2f)
                {
                    _animator.SetBool(AnimationID.RunID, false);
                }

            }


        }

        /// <summary>
        /// �Ų���
        /// </summary>
        private void SetCharacterFootSound()
        {
            if(_characterIsOnGroud && _animator.GetFloat(AnimationID.MovementID)>0.5f && _animator.AnimationAtTag("Motion"))
            {
                //����motion��ǩ��ʾ�ƶ�����
                _nextFootTime -= Time.deltaTime;
                if(_nextFootTime < 0f)
                {
                    PlayFootSound();
                }
            }
            else
            {
                _nextFootTime = 0f;
            }
        }
        private void PlayFootSound()
        {
            //���ŽŲ���
            GamePoolManager.Instance.TryGetPoolItem("FootSound", transform.position, Quaternion.identity);
            //������һ�β��ŽŲ�����ʱ��
            _nextFootTime = (_animator.GetFloat(AnimationID.MovementID) > 1.1f) ? _fastFootTime : _slowFootTime;
        }

    }

}
