using GGG.Tool;
using MyARPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.Movement
{
    public class PlayerMovementController : CharacterMovementControllerBase
    {
        //角度相关
        private Transform _mainCamera;                                  //主摄像机，因为我们移动时转向的方向要参考摄像机的朝向
        private float _rotationAngle;                                   //用于记录本帧角色要转向的角度
        private float _angleVelocity = 0f;                              //角色转向的速度
        [SerializeField] private float _rotationSmoothTime;             //角色转向平滑过渡时间
        private Vector3 _characterTargetDirection;                      //角色的目标朝向，用于计算角度增量

        //脚步声间隔
        private float _nextFootTime;                                    //下一次播放脚步声的时间
        [SerializeField] private float _slowFootTime;
        [SerializeField] private float _fastFootTime;

        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            //设置行走动画，rootmotion会推动游戏对象的position移动
            UpdateAnimation();
            //因为转向受到玩家的移动影响，所以放到lateupdate之后处理角度是比较合适的
            CharacterRotationControl();
        }

        /// <summary>
        /// 人物行走动画切换逻辑
        /// </summary>
        private void UpdateAnimation()
        {
            if (!_characterIsOnGroud) return;
            
            //设置hasinput 动画变量
            _animator.SetBool(AnimationID.HasInputID, InputManager.Instance.Movement != Vector2.zero);

            Debug.Log("test="+InputManager.Instance.Movement.normalized);
            if (_animator.GetBool(AnimationID.HasInputID))
            {
                //设置run动画变量
                if (InputManager.Instance.Run)
                {
                    _animator.SetBool(AnimationID.RunID, true);
                }
                //设置movement动画变量，用于控制行走和奔跑的blend tree
                //sqrMagnitude是向量模的平方，这里算出来最大就是了，也没必要使用开方的费时操作。
                _animator.SetFloat(AnimationID.MovementID, _animator.GetBool(AnimationID.RunID) ? 2f : InputManager.Instance.Movement.sqrMagnitude, 0.25f, Time.deltaTime);

                //播放脚步声
                SetCharacterFootSound();
            }
            else
            {
                //如果没有移动输入，就讲移动速度慢慢过度到0
                //public void SetFloat(int id, float value, float dampTime, float deltaTime);
                //damptime是过渡时间
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
        /// 人物身体的旋转控制
        /// </summary>
        private void CharacterRotationControl()
        {
            if (!_characterIsOnGroud ) return;

            if (_animator.GetBool(AnimationID.HasInputID))
            {
                //获取摄像机的朝向+玩家控制人物移动产生的角度旋转
                _rotationAngle = _mainCamera.eulerAngles.y + Mathf.Atan2(InputManager.Instance.Movement.x, InputManager.Instance.Movement.y) * Mathf.Rad2Deg ;
            }

            if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))//比如说被打的时候就不能旋转
            {
                //if (_animator.GetFloat(AnimationID.DeltaAngleID) < -135f) return;
                //if (_animator.GetFloat(AnimationID.DeltaAngleID) > 135f) return;

                //将角色以y为轴进行平滑旋转
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle, ref _angleVelocity, _rotationSmoothTime);
                
                //当前角色的forward绕y轴旋转_rotationAngle，得到角色的目标方向。
                _characterTargetDirection = Quaternion.Euler(0f, _rotationAngle, 0f) * Vector3.forward;
            }

            //设置动画参数中的角度增量：当前的朝向和目标朝向的角度增量
            //_animator.SetFloat(AnimationID.DeltaAngleID, DevelopmentToos.GetDeltaAngle(transform,_characterTargetDirection.normalized));
        }

        /// <summary>
        /// 设置角色的脚步声什么时候播放
        /// </summary>
        private void SetCharacterFootSound()
        {
            if(_characterIsOnGroud && _animator.GetFloat(AnimationID.MovementID)>0.5f && _animator.AnimationAtTag("Motion"))
            {
                //这里motion标签表示移动动画
                _nextFootTime -= Time.deltaTime;
                if(_nextFootTime < 0f)
                {
                    //播放脚步声
                    GamePoolManager.Instance.TryGetPoolItem("FootSound", transform.position, Quaternion.identity);
                    //设置下一次播放脚步声的时间
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
