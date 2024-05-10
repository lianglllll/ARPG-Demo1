using GGG.Tool;
using MyARPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.Movement
{
    public class PlayerMovementController : CharacterMovementControllerBase
    {
        //�Ƕ����
        private Transform _mainCamera;                                  //�����������Ϊ�����ƶ�ʱת��ķ���Ҫ�ο�������ĳ���
        private float _rotationAngle;                                   //���ڼ�¼��֡��ɫҪת��ĽǶ�
        private float _angleVelocity = 0f;                              //��ɫת����ٶ�
        [SerializeField] private float _rotationSmoothTime;             //��ɫת��ƽ������ʱ��
        private Vector3 _characterTargetDirection;                      //��ɫ��Ŀ�곯�����ڼ���Ƕ�����

        //�Ų������
        private float _nextFootTime;                                    //��һ�β��ŽŲ�����ʱ��
        [SerializeField] private float _slowFootTime;
        [SerializeField] private float _fastFootTime;

        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            //�������߶�����rootmotion���ƶ���Ϸ�����position�ƶ�
            UpdateAnimation();
            //��Ϊת���ܵ���ҵ��ƶ�Ӱ�죬���Էŵ�lateupdate֮����Ƕ��ǱȽϺ��ʵ�
            CharacterRotationControl();
        }

        /// <summary>
        /// �������߶����л��߼�
        /// </summary>
        private void UpdateAnimation()
        {
            if (!_characterIsOnGroud) return;
            
            //����hasinput ��������
            _animator.SetBool(AnimationID.HasInputID, InputManager.Instance.Movement != Vector2.zero);

            Debug.Log("test="+InputManager.Instance.Movement.normalized);
            if (_animator.GetBool(AnimationID.HasInputID))
            {
                //����run��������
                if (InputManager.Instance.Run)
                {
                    _animator.SetBool(AnimationID.RunID, true);
                }
                //����movement�������������ڿ������ߺͱ��ܵ�blend tree
                //sqrMagnitude������ģ��ƽ��������������������ˣ�Ҳû��Ҫʹ�ÿ����ķ�ʱ������
                _animator.SetFloat(AnimationID.MovementID, _animator.GetBool(AnimationID.RunID) ? 2f : InputManager.Instance.Movement.sqrMagnitude, 0.25f, Time.deltaTime);

                //���ŽŲ���
                SetCharacterFootSound();
            }
            else
            {
                //���û���ƶ����룬�ͽ��ƶ��ٶ��������ȵ�0
                //public void SetFloat(int id, float value, float dampTime, float deltaTime);
                //damptime�ǹ���ʱ��
                _animator.SetFloat(AnimationID.MovementID, 0f, 0.25f, Time.deltaTime);
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
        /// �����������ת����
        /// </summary>
        private void CharacterRotationControl()
        {
            if (!_characterIsOnGroud ) return;

            if (_animator.GetBool(AnimationID.HasInputID))
            {
                //��ȡ������ĳ���+��ҿ��������ƶ������ĽǶ���ת
                _rotationAngle = _mainCamera.eulerAngles.y + Mathf.Atan2(InputManager.Instance.Movement.x, InputManager.Instance.Movement.y) * Mathf.Rad2Deg ;
            }

            if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))//����˵�����ʱ��Ͳ�����ת
            {
                //if (_animator.GetFloat(AnimationID.DeltaAngleID) < -135f) return;
                //if (_animator.GetFloat(AnimationID.DeltaAngleID) > 135f) return;

                //����ɫ��yΪ�����ƽ����ת
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle, ref _angleVelocity, _rotationSmoothTime);
                
                //��ǰ��ɫ��forward��y����ת_rotationAngle���õ���ɫ��Ŀ�귽��
                _characterTargetDirection = Quaternion.Euler(0f, _rotationAngle, 0f) * Vector3.forward;
            }

            //���ö��������еĽǶ���������ǰ�ĳ����Ŀ�곯��ĽǶ�����
            //_animator.SetFloat(AnimationID.DeltaAngleID, DevelopmentToos.GetDeltaAngle(transform,_characterTargetDirection.normalized));
        }

        /// <summary>
        /// ���ý�ɫ�ĽŲ���ʲôʱ�򲥷�
        /// </summary>
        private void SetCharacterFootSound()
        {
            if(_characterIsOnGroud && _animator.GetFloat(AnimationID.MovementID)>0.5f && _animator.AnimationAtTag("Motion"))
            {
                //����motion��ǩ��ʾ�ƶ�����
                _nextFootTime -= Time.deltaTime;
                if(_nextFootTime < 0f)
                {
                    //���ŽŲ���
                    GamePoolManager.Instance.TryGetPoolItem("FootSound", transform.position, Quaternion.identity);
                    //������һ�β��ŽŲ�����ʱ��
                    _nextFootTime = (_animator.GetFloat(AnimationID.MovementID) > 1.1f) ? _fastFootTime : _slowFootTime;
                }
            }
            else
            {
                _nextFootTime = 0f;
            }
        }

    }

}
