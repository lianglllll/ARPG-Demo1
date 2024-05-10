using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Movement
{

    /// <summary>
    /// 1.看着敌人
    /// 2.切换移动动画
    /// </summary>
    public class EnemyMovementController : CharacterMovementControllerBase
    {

        //1.动画控制
        //2.在移动动画播放的时候，让ai看着玩家的方向！
        //3.在非移动状态下，我们就把这个动画控制器设置为0


        private bool _applyMovement;            //是否允许控制


        protected override void Start()
        {
            base.Start();
            SetApplyMovement(true);
        }

        protected override void Update()
        {
            base.Update();
            LookTargetDirection();
            DrawDirection();//测试用
        }

        /// <summary>
        /// 看向玩家
        /// </summary>
        private void  LookTargetDirection()
        {
            //1.我们只希望AI在移动的时候才看着玩家
            if (_animator.AnimationAtTag("Motion"))
            {
                transform.Look(EnemyManager.Instance.GetMainPlayer().position,500f);
            }
        }

        /// <summary>
        /// 设置动画移动参数
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
        /// 设置_applyMovement
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
