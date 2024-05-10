using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Movement
{

    /// <summary>
    /// 1.���ŵ���
    /// 2.�л��ƶ�����
    /// </summary>
    public class EnemyMovementController : CharacterMovementControllerBase
    {

        //1.��������
        //2.���ƶ��������ŵ�ʱ����ai������ҵķ���
        //3.�ڷ��ƶ�״̬�£����ǾͰ������������������Ϊ0


        private bool _applyMovement;            //�Ƿ��������


        protected override void Start()
        {
            base.Start();
            SetApplyMovement(true);
        }

        protected override void Update()
        {
            base.Update();
            LookTargetDirection();
            DrawDirection();//������
        }

        /// <summary>
        /// �������
        /// </summary>
        private void  LookTargetDirection()
        {
            //1.����ֻϣ��AI���ƶ���ʱ��ſ������
            if (_animator.AnimationAtTag("Motion"))
            {
                transform.Look(EnemyManager.Instance.GetMainPlayer().position,500f);
            }
        }

        /// <summary>
        /// ���ö����ƶ�����
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void SetAnimatorMovementValue(float horizontal,float vertical)
        {
            if (_applyMovement)
            {
                _animator.SetBool(AnimationID.HasInputID, true);
                _animator.SetFloat(AnimationID.LockID, 1f);
                _animator.SetFloat(AnimationID.HorizontalID, horizontal, 0.2f, Time.deltaTime);
                _animator.SetFloat(AnimationID.VerticalID, vertical, 0.2f, Time.deltaTime);
            }
            else
            {
                _animator.SetBool(AnimationID.HasInputID, false);
                _animator.SetFloat(AnimationID.LockID, 0f);
                _animator.SetFloat(AnimationID.HorizontalID, 0f, 0.2f, Time.deltaTime);
                _animator.SetFloat(AnimationID.VerticalID, 0f, 0.2f, Time.deltaTime);
            }
        }

        /// <summary>
        /// ����_applyMovement
        /// </summary>
        /// <param name="apply"></param>
        public void SetApplyMovement(bool apply)
        {
            _applyMovement = apply;
        }

        /// <summary>
        /// test
        /// </summary>
        private void DrawDirection()
        {
            Debug.DrawRay(transform.position + (transform.up * 0.7f), (EnemyManager.Instance.GetMainPlayer().position - transform.position), Color.yellow);
        }

        public void EnableCharacterController(bool enable)
        {
            _controller.enabled = enable;
        }

    }
}
