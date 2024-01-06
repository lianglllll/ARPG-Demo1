using GGG.Tool;
using MyARPG.ComboData;
using MyARPG.HealthData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.Health
{
    public interface IDamage
    {
        void CharacterNormalDamage(string hitName, string parryName, float damage, Transform attacker,DamagedType damagedType);
        void BeginFinished(string hitName, Transform attacker);
        void FinishDamage(float damage, DamagedType type, bool isLastDamage);

    }

    public abstract class CharacterHealthBase : MonoBehaviour,IDamage
    {
        protected Animator _animator;
        protected Transform _currentAttacker;                                       //当前的攻击者，打我的那个叼毛
        protected IFX _fx;                                                          //控制粒子的脚本

        [SerializeField, Header("角色生命值信息模板")]
        protected CharacterHealthDataSO _healthInfo;                                //初始生命模板
        protected CharacterHealthDataSO _characterHealthData;                       //实际的生命面板

        protected float FinishDamageaccumulate = 0f;


        public CharacterHealthDataSO CharacterHealthData => _characterHealthData;
        public bool CharacterIsDeath() => _characterHealthData._isDeath;
        public bool CanFinish() => _characterHealthData.CurrentHP <= 30f;


        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterHealthData = ScriptableObject.Instantiate(_healthInfo);
            _characterHealthData.InitCharacterHealthInfo();
            _fx = transform.Find("CharacterHitFx").GetComponent<IFX>();
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Update()
        {
            OnHitLookAtAttacker();
        }

        /// <summary>
        /// 设置攻击者
        /// </summary>
        /// <param name="attacker"></param>
        private void SetAttacker(Transform attacker)
        {
            if(_currentAttacker == null || _currentAttacker != attacker)
            {
                _currentAttacker = attacker;
            }
        }

        /// <summary>
        /// 看向攻击者
        /// </summary>
        private void OnHitLookAtAttacker()
        {
            if (_currentAttacker == null) return;
            if(_animator.AnimationAtTag("Hit")||  _animator.AnimationAtTag("Parry") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f){
                transform.Look(_currentAttacker.position, 50f);
            }
        }

        /// <summary>
        /// 扣除生命值
        /// </summary>
        /// <param name="damage"></param>
        protected virtual void TakeDamage(float damage)
        {
            _characterHealthData.Damage(damage);
            GameEventManager.Instance.CallEvent<CharacterHealthBase>("UpdateHealthInfo", this);
        }

        /// <summary>
        /// 扣体力值
        /// </summary>
        /// <param name="damage"></param>
        protected virtual void TakeStrength(float damage)
        {
            _characterHealthData.DamageToStrength(damage);
            GameEventManager.Instance.CallEvent<CharacterHealthBase>("UpdateHealthInfo", this);
        }

        /// <summary>
        /// 角色的受伤行为,播放动作什么的(给子类重写的)
        /// </summary>
        /// <param name="hitName">受伤动画</param>
        /// <param name="parrName">格挡动画</param>
        protected virtual void CharacterHitAction(float damage, string hitName, string parrName, DamagedType type)
        {

        }

        /// <summary>
        /// 播放死亡动画
        /// </summary>
        protected virtual void PlayDeadAnimation()
        {
            if (!_animator.AnimationAtTag("FinishHit"))
            {
                _animator.Play("Dead1", 0, 0f);
            }
        } 

        /// <summary>
        /// 播放受伤音效
        /// </summary>
        /// <param name="type"></param>
        protected void PlayHitClip(DamagedType type)
        {
            switch (type)
            {
                case DamagedType.PUNCH:
                    GamePoolManager.Instance.TryGetPoolItem("HitSound", transform.position, Quaternion.identity);
                    break;
                case DamagedType.WEAPON:
                    GamePoolManager.Instance.TryGetPoolItem("SwordHitSound", transform.position, Quaternion.identity);
                    break;
            }
        }

        /// <summary>
        /// 收到普通攻击
        /// </summary>
        /// <param name="hitName"></param>
        /// <param name="parryName"></param>
        /// <param name="damage"></param>
        /// <param name="attacker"></param>
        /// <param name="damagedType"></param>
        public void CharacterNormalDamage(string hitName, string parryName, float damage, Transform attacker, DamagedType damagedType)
        {
            SetAttacker(attacker);
            CharacterHitAction(damage, hitName, parryName, damagedType);
        }

        /// <summary>
        /// 开始被处决，我们要准备好被处决的动画
        /// </summary>
        /// <param name="hitName"></param>
        /// <param name="attacker"></param>
        public void BeginFinished(string hitName, Transform attacker)
        {
            SetAttacker(attacker);
            _animator.Play(hitName);
        }

        /// <summary>
        /// 处决期间受到的伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="type">伤害类型</param>
        /// <param name="isLastDamage">是否是处决的最后一下</param>
        public void FinishDamage(float damage, DamagedType type,bool isLastDamage)
        {
            FinishDamageaccumulate += damage;
            //播放音效
            PlayHitClip(type);
            if (isLastDamage)
            {
                TakeDamage(damage);
                FinishDamageaccumulate = 0;
            }
        }
    }
}

