using GGG.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MyARPG
{
    /// <summary>
    /// 角色移动基类
    /// </summary>
    
    [RequireComponent(typeof(CharacterController))]                         //强制需要有一个charactercontroller，目的是为了防止添加角色控制器
    public abstract class CharacterMovementControllerBase : MonoBehaviour
    {
        protected CharacterController _controller;
        protected Animator _animator;

        //地面检测
        protected bool _characterIsOnGroud;                                                         //角色是否在地面上                      
        [SerializeField,Header("地面检测")] protected float _groudDetectionPositionOffset;    //为了更好控制检测器的位置，这里添加一个offset
        [SerializeField] protected float _detectionRang;                                            //检测范围
        [SerializeField] protected LayerMask _whatIsGround;                                         //检测场景，只去检测被标记为groud的layer，其他的就忽略掉

        //重力
        protected readonly float CharacterGravity = -9.8f;              //重力加速度
        protected float _characterVerticalVelocity;                     //用于更新角色Y轴的速度(可以应用于重力和跳跃高度,砍敌人一刀，敌人往天上飞也可以用这个变量实现)
        protected readonly float _characterVerticalMaxVelocity = 54f;   //角色在低于这个值的时候，才需要应用重力
        protected Vector3 _characterVerticalDireciton;                  //角色的Y轴移动方向，因为是通过charactercontroller的move函数来实现重力。所以把_characterVerticalVelocity，应用到这个向量的Y值里面去更新
        protected float _fallOutDeltaTime;
        protected float _fallOutTime = 0.15f;                           //防止角色下楼梯的时候鬼畜，比如下楼梯时播放跌落动画。这个0.15秒用于缓冲，判断角色是不是在下楼梯
        protected bool _isEnableGravity;                                //是否启用重力

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
        /// 使用动画来驱动移动
        /// </summary>
        protected virtual void OnAnimatorMove()
        {
            _animator.ApplyBuiltinRootMotion();                    //告诉 Animator 使用动画中的 Root Motion 来影响游戏对象的移动。
            UpdateCharacterMoveDirection(_animator.deltaPosition);//animator.deltaPosition上一帧动画的位置变化，所以你的动画需要带位移才能这样用
        }




        //地面检测
        private bool GroundDetection()
        {
            //检测的中心点
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groudDetectionPositionOffset, transform.position.z);
            return Physics.CheckSphere(detectionPosition, _detectionRang, _whatIsGround, QueryTriggerInteraction.Ignore);
        }
        //设置受重力影响的速度大小
        private void SetCharacterGracity()
        {
            _characterIsOnGroud = GroundDetection();

            if (_characterIsOnGroud)
            {
                //如果角色在地面上需要重置FallOutTime
                _fallOutDeltaTime = _fallOutTime;
                //重置角色垂直速度
                if(_characterVerticalVelocity < 0f)
                {
                    _characterVerticalVelocity = -2f;//如果这里不给他固定死，那么它会一直累积。当你第二次跳跃或者从高处跌落，那么你的下降速度就会很快。不会从慢到快。
                    //固定-2，那么你第二次高出跌落，那么你的垂直速度就是从-2f开始计算
                    //非固定，在地面还在一直累计，-100f或者-500f,这时候你在第二次高处跌落，那么你的垂直速度就是从-500开始计算
                }
            }
            else
            {    
                //不在地面上
                if (_fallOutDeltaTime > 0)
                {
                    _fallOutDeltaTime -= Time.deltaTime;//等待0.15秒用于缓冲，这个0.15秒用于帮助角色从较低的高度差下落(比如两节楼梯)，这么短的时间就没有必要播放下落的动作
                }
                else
                {
                    //说明过来0.15秒角色还没有落地，可能不是在下楼梯，那么就有必要播放下落动画了

                }
                
                //设置重力  //todo
                if((-_characterVerticalVelocity) < _characterVerticalMaxVelocity && _isEnableGravity)
                {
                    _characterVerticalVelocity += CharacterGravity * Time.deltaTime;
                }
            }
        }
        private void UpdateCharacterGravity()
        {
            if (!_isEnableGravity) return;
            _characterVerticalDireciton.Set(0, _characterVerticalVelocity, 0);
            _controller.Move(_characterVerticalDireciton * Time.deltaTime);
        }

        //坡道检测
        private Vector3 SlopResetDirection(Vector3 moveDireciton)
        {
            //检测角色限制是否在坡上移动，防止角色下坡速度过快时，变成弹力球
            if(Physics.Raycast(transform.position+(transform.up*5f),Vector3.down,out var hit,_controller.height*0.85f,_whatIsGround,QueryTriggerInteraction.Ignore)){
                //点积==0 说明两个向量是垂直的，浮点值不会完全相等，只会无限接近
                //hit.normal是射线打到物体的法线
                if(Vector3.Dot(Vector3.up,hit.normal) != 0)
                {
                    //ProjectOnPlane:向量投影到normal对应的平面上。
                    //比如：vec:(1,2,3) nor:(0,1,0)  得到的投影就是（1,0,3）
                    return Vector3.ProjectOnPlane(moveDireciton, hit.normal);
                }
            }
            return moveDireciton;
        }


        //移动
        protected void UpdateCharacterMoveDirection(Vector3 diretion)
        {
            _moveDirection = SlopResetDirection(diretion);
            _controller.Move(_moveDirection * Time.deltaTime);
        }

        //可视化,测试用
        private void OnDrawGizmos()
        {
            //检测的中心点
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groudDetectionPositionOffset, transform.position.z);
            Gizmos.DrawWireSphere(detectionPosition, _detectionRang);
        }


        private void EnableCharacterGravity(bool enable)
        {
            _isEnableGravity = enable;
            _characterVerticalVelocity = (enable == true) ? -2f : 0f;
        }


    }
}
