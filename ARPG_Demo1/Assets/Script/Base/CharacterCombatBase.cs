using GGG.Tool;
using MyARPG.ComboData;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Combat
{
    public class CharacterCombatBase : MonoBehaviour
    {
        //玩家和ai它们的攻击触发是相同的
        //伤害触发也是相同的
        //它们的基础组合技也是差不多的
        //组合技的信息更新

        [Header("动画参数")]
        protected Animator _animator;
        [SerializeField] protected float _animationCrossNormalTime;                     //动画转换的混合时间

        [Header("基础的招式表")]
        [SerializeField] protected CharacterComboSO _baseCombo;                         //基础攻击招式表
        [SerializeField] protected CharacterComboSO _finishCombo;                       //处决表
        protected CharacterComboSO _currentCombo;                                       //当前的招式表
        protected int _currentComboIndex;                                               //当前招式表中的招式索引
        protected float _maxColdTime;                                                   //攻击的最大间隔时间
        protected bool _canAttackInput;                                                 //是否允许输入攻击信号
        protected int _hitIndex;                                                        //当前招式里的动作索引，用于找hit和parry动画
        protected int _currentComboAttackCount;                                         //当前连击数量,用于变招用：轻轻重、轻轻轻、轻轻轻轻轻
        protected ComboDataType _currentComboDataType;


        //处决
        protected int _finishComboIndex;
        protected bool _canFinish;


        [Header("敌人信息")]
        [SerializeField] protected Transform _currentEnemy;


        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            _canAttackInput = true;
            _currentCombo = _baseCombo;
            _canFinish = false;
        }

        protected virtual void Update()
        {
            MatchPosition();
            LookTargetOnAttack();
            OnEndAttack();
        }


        #region 位置同步,找到和目标适合播放动画的位置

        protected virtual void MatchPosition()
        {
            if (_currentEnemy == null) return;
            if (_animator == null) return;
            if (_animator.AnimationAtTag("Dead")) return;

            if (_animator.AnimationAtTag("Attack"))
            {
                var timer = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (timer > 0.35f) return;
                if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return;
                RunningMatch(_currentCombo, _currentComboIndex, 0f, 0.45f);
            }
        }

        /// <summary>
        /// 位置匹配
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        protected void RunningMatch(CharacterComboSO combo, int index, float startTime = 0f, float endTime = 0.1f)
        {
            if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))//当前不在匹配同时不在过度状态
            {
                _animator.MatchTarget(_currentEnemy.position + (-transform.forward * combo.TryGetComboPositionOffset(index)), Quaternion.identity,
                    AvatarTarget.Body, new MatchTargetWeightMask(Vector3.one, 0f), startTime, endTime);
            }

        }

        #endregion

        #region 基础攻击输入
        protected virtual void CharacterBaseAtackInput()
        {

        }
        /// <summary>
        /// 改变连招表
        /// </summary>
        /// <param name="combo"></param>
        protected void ChangeCombo(CharacterComboSO combo)
        {
            if (_currentCombo != combo)
            {
                _currentCombo = combo;
                ResetComboInfo();
            }
        }
        #endregion

        #region 招式执行，动画播放
        protected virtual void ExecuteComboAction()
        {
            //更新当前招式的动作HitIndex索引值
            _hitIndex = 0;
            _currentComboAttackCount += (_currentCombo == _baseCombo) ? 1 : 0;
            _currentComboDataType = _currentCombo.GetComboDataType(_currentComboIndex);

            _maxColdTime = _currentCombo.TryGetComboColdTime(_currentComboIndex);
            _animator.CrossFadeInFixedTime(_currentCombo.TryGetOneComboAction(_currentComboIndex), 0.15555f, 0, 0f);//播放动作
            GameTimerManager.Instance.TryUseOneTimer(_maxColdTime, UpdateComboInfo);        //注意这个函数UpdateComboInfo，如果冷却事件过断，可能会导致_currentComboIndex的++操作导致越界
            _canAttackInput = false; //禁用攻击输入，等到上面哪个动画播放完才打开

        }
        #endregion

        #region 攻击时看着目标
        protected void LookTargetOnAttack()
        {
            if (_currentEnemy == null) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 5f) return;
            //攻击的时候，看向敌人
            if (_animator.AnimationAtTag("Attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                transform.Look(_currentEnemy.position, 1000f);
            }
        }

        #endregion

        #region 攻击事件，动画触发的
        /// <summary>
        /// 动画事件触发的攻击事件
        /// </summary>
        protected void ATK()
        {
            TriggerDamage();
            UpdateHitIndex();
            PlayATKSound(_currentComboDataType);
        }
        public void PlayATKSound(ComboDataType type)
        {
            switch (type)
            {
                case ComboDataType.PUNCH:
                case ComboDataType.TICK:
                    GamePoolManager.Instance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
                    break;
                case ComboDataType.WEAPON:
                    GamePoolManager.Instance.TryGetPoolItem("SwordSound", transform.position, Quaternion.identity);
                    break;
            }
        }

        protected void LastFinishATK()
        {
            TriggerFinishDamage();
            UpdateHitIndex();
            GamePoolManager.Instance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
        }


        #endregion

        #region 伤害触发


        /// <summary>
        /// 每个招式中的动作来触发一次
        /// </summary>
        protected void TriggerDamage()
        {

            //1.我们要确保有目标
            if (_currentEnemy == null) return;
            //2.要确保敌人处于我们可触发伤害的距离和角度
            if (Vector3.Dot(transform.forward, DevelopmentToos.DirectionForTarget(_currentEnemy, transform)) > 0.85f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 1.3f) return;

            //3.去呼叫事件中心，帮我调用，触发伤害这个函数
            if (_animator.AnimationAtTag("Attack"))
            {
                GameEventManager.Instance.CallEvent("OnCharacterHitEvent",
                    _currentCombo.TryGetComboDamage(_currentComboIndex),
                    _currentCombo.TryGetOneHitName(_currentComboIndex, _hitIndex),
                    _currentCombo.TryGetOneParryName(_currentComboIndex, _hitIndex),
                    transform, _currentEnemy,_currentCombo.GetComboDataType(_currentComboIndex));
                //这里传的受伤动画是单个动画片段
            }
            else
            {
                //处决动作，暗杀动作
                //处决是一个完整的被处决动作，同一个动画期间，会触发多个伤害
                GameEventManager.Instance.CallEvent("OnFinishDamage", _currentEnemy);
            }

        }

        protected void TriggerFinishDamage()
        {
            //1.我们要确保有目标
            if (_currentEnemy == null) return;
            //2.要确保敌人处于我们可触发伤害的距离和角度
            if (Vector3.Dot(transform.forward, DevelopmentToos.DirectionForTarget(_currentEnemy, transform)) > 0.85f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 1.3f) return;
            GameEventManager.Instance.CallEvent("OnLastFinishDamage", _finishCombo.TryGetComboDamage(_finishComboIndex), _currentEnemy);
        }

        #endregion

        #region 更新招式信息

        //timer任务完成的回调:动画播放完就执行
        protected virtual void UpdateComboInfo()
        {
            _currentComboIndex++;
            if (_currentComboIndex == _currentCombo.TryGetComboMaxCount())
            {
                //如果当前的攻击动作已经执行到最后一个动作(招式)
                _currentComboIndex = 0;
            }
            _maxColdTime = 0;
            _canAttackInput = true;
        }

        protected void UpdateHitIndex()
        {
            _hitIndex++;
            if (_hitIndex >= _currentCombo.TryGetHitOrParryMaxCount(_currentComboIndex))
            {
                _hitIndex = 0;
            }

        }
        #endregion

        #region 重置招式信息

        protected void ResetComboInfo()
        {
            _currentComboIndex = 0;
            _maxColdTime = 0f;
            _hitIndex = 0;
        }

        /// <summary>
        /// 攻击动作结束后，我们重置一下攻击索引
        /// </summary>
        protected void OnEndAttack()
        {
            //动作在移动&&可以接收输入的话
            //说明我在上一段攻击结束之后，我已经不继续攻击了，我要开始动了
            if (_animator.AnimationAtTag("Motion") && _canAttackInput)
            {
                ResetComboInfo();
                _currentComboAttackCount = 0;
            }
        }


        #endregion





    }
}
