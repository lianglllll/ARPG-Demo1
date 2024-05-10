using GGG.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MyARPG
{
    /// <summary>
    /// ��ɫ�ƶ�����
    /// </summary>
    
    [RequireComponent(typeof(CharacterController))]                         //ǿ����Ҫ��һ��charactercontroller��Ŀ����Ϊ�˷�ֹ��ӽ�ɫ������
    public abstract class CharacterMovementControllerBase : MonoBehaviour
    {
        protected CharacterController _controller;
        protected Animator _animator;

        //������
        protected bool _characterIsOnGroud;                                                         //��ɫ�Ƿ��ڵ�����                      
        [SerializeField,Header("������")] protected float _groudDetectionPositionOffset;    //Ϊ�˸��ÿ��Ƽ������λ�ã��������һ��offset
        [SerializeField] protected float _detectionRang;                                            //��ⷶΧ
        [SerializeField] protected LayerMask _whatIsGround;                                         //��ⳡ����ֻȥ��ⱻ���Ϊgroud��layer�������ľͺ��Ե�

        //����
        protected readonly float CharacterGravity = -9.8f;              //�������ٶ�
        protected float _characterVerticalVelocity;                     //���ڸ��½�ɫY����ٶ�(����Ӧ������������Ծ�߶�,������һ�������������Ϸ�Ҳ�������������ʵ��)
        protected readonly float _characterVerticalMaxVelocity = 54f;   //��ɫ�ڵ������ֵ��ʱ�򣬲���ҪӦ������
        protected Vector3 _characterVerticalDireciton;                  //��ɫ��Y���ƶ�������Ϊ��ͨ��charactercontroller��move������ʵ�����������԰�_characterVerticalVelocity��Ӧ�õ����������Yֵ����ȥ����
        protected float _fallOutDeltaTime;                              //�����ʱ��
        protected float _fallOutTime = 0.15f;                           //��ֹ��ɫ��¥�ݵ�ʱ����󣬱�����¥��ʱ���ŵ��䶯�������0.15�����ڻ��壬�жϽ�ɫ�ǲ�������¥��
        protected bool _isEnableGravity;                                //�Ƿ���������

        //��ɫ�ƶ�
        protected Vector3 _moveDirection;                               //tmp

        protected virtual void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void OnEnable()
        {
            GameEventManager.Instance.AddEventListening<bool>("EnableCharacterGravity", EnableCharacterGravity);
        }

        protected virtual void OnDisable()
        {
            GameEventManager.Instance.RemoveEvent<bool>("EnableCharacterGravity", EnableCharacterGravity);
        }

        protected virtual void Start()
        {
            _fallOutDeltaTime = _fallOutTime;
            _isEnableGravity = true;
        }

        protected virtual void Update()
        {
            SetCharacterGracity();
            UpdateCharacterGravity();
        }

        /// <summary>
        /// ���ӻ�,������
        /// </summary>
        private void OnDrawGizmos()
        {
            //�������ĵ�
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groudDetectionPositionOffset, transform.position.z);
            Gizmos.DrawWireSphere(detectionPosition, _detectionRang);
        }

        /// <summary>
        /// ʹ�ö����������ƶ�
        /// </summary>
        protected virtual void OnAnimatorMove()
        {
            _animator.ApplyBuiltinRootMotion();                    //���� Animator ʹ�ö����е� Root Motion ��Ӱ����Ϸ������ƶ���
            UpdateCharacterMoveDirection(_animator.deltaPosition);//animator.deltaPosition��һ֡������λ�ñ仯��������Ķ�����Ҫ��λ�Ʋ���������
        }

        /// <summary>
        /// �ƶ�
        /// </summary>
        /// <param name="diretion"></param>
        protected void UpdateCharacterMoveDirection(Vector3 diretion)
        {
            _moveDirection = SlopResetDirection(diretion);
            _controller.Move(_moveDirection * Time.deltaTime);
        }

        /// <summary>
        /// �µ����
        /// </summary>
        /// <param name="moveDireciton"></param>
        /// <returns></returns>
        private Vector3 SlopResetDirection(Vector3 moveDireciton)
        {
            //����ɫ�����Ƿ��������ƶ�����ֹ��ɫ�����ٶȹ���ʱ����ɵ�����
            if (Physics.Raycast(transform.position + (transform.up *0.5f), Vector3.down, out var hit, _controller.height * 0.85f, _whatIsGround, QueryTriggerInteraction.Ignore))
            {
                if (hit.normal != Vector3.up)
                {
                    //ProjectOnPlane:����ͶӰ��normal��Ӧ��ƽ���ϡ�
                    //���磺vec:(1,2,3) nor:(0,1,0)  �õ���ͶӰ���ǣ�1,0,3��
                    return Vector3.ProjectOnPlane(moveDireciton, hit.normal);
                }
            }
            return moveDireciton;
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        private bool GroundDetection()
        {
            //�������ĵ�
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groudDetectionPositionOffset, transform.position.z);
            return Physics.CheckSphere(detectionPosition, _detectionRang, _whatIsGround, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// ���ý�ɫ������Ӱ����ٶ�
        /// </summary>
        private void SetCharacterGracity()
        {
            if (!_isEnableGravity) return;

            _characterIsOnGroud = GroundDetection();

            if (_characterIsOnGroud)
            {
                //�����ɫ�ڵ�������Ҫ����FallOutTime
                _fallOutDeltaTime = _fallOutTime;
                //���ý�ɫ��ֱ�ٶ�
                if(_characterVerticalVelocity < 0f)
                {
                    _characterVerticalVelocity = -2f;//������ﲻ�����̶�������ô����һֱ�ۻ�������ڶ�����Ծ���ߴӸߴ����䣬��ô����½��ٶȾͻ�ܿ졣����������졣
                    //�̶�-2����ô��ڶ��θ߳����䣬��ô��Ĵ�ֱ�ٶȾ��Ǵ�-2f��ʼ����
                    //�ǹ̶����ڵ��滹��һֱ�ۼƣ�-100f����-500f,��ʱ�����ڵڶ��θߴ����䣬��ô��Ĵ�ֱ�ٶȾ��Ǵ�-500��ʼ����
                }
            }
            else
            {    
                //���ڵ�����
                if (_fallOutDeltaTime > 0)
                {
                    _fallOutDeltaTime -= Time.deltaTime;//�ȴ�0.15�����ڻ��壬���0.15�����ڰ�����ɫ�ӽϵ͵ĸ߶Ȳ�����(��������¥��)����ô�̵�ʱ���û�б�Ҫ��������Ķ���
                }
                else
                {
                    //˵������0.15���ɫ��û����أ����ܲ�������¥�ݣ���ô���б�Ҫ�������䶯����

                }

                //����������Ӱ���´�ֱ�����ϵ��ٶ�
                //����һֱ�ۼӣ�ֱ���ٶȵĴ�С ���� ���Ĵ�ֱ�ٶȵĴ�С
                if((-_characterVerticalVelocity) < _characterVerticalMaxVelocity)
                {
                    _characterVerticalVelocity += CharacterGravity * Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// ���½�ɫ�����������ϵ�λ��
        /// </summary>
        private void UpdateCharacterGravity()
        {
            if (!_isEnableGravity) return;
            _characterVerticalDireciton.Set(0, _characterVerticalVelocity, 0);
            _controller.Move(_characterVerticalDireciton * Time.deltaTime);
        }

        /// <summary>
        /// �����Ƿ���������
        /// </summary>
        /// <param name="enable"></param>
        private void EnableCharacterGravity(bool enable)
        {
            _isEnableGravity = enable;
            _characterVerticalVelocity = (enable == true) ? -2f : 0f;
        }

    }
}
