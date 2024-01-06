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
        protected Transform _currentAttacker;                                       //��ǰ�Ĺ����ߣ����ҵ��Ǹ���ë
        protected IFX _fx;                                                          //�������ӵĽű�

        [SerializeField, Header("��ɫ����ֵ��Ϣģ��")]
        protected CharacterHealthDataSO _healthInfo;                                //��ʼ����ģ��
        protected CharacterHealthDataSO _characterHealthData;                       //ʵ�ʵ��������

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
        /// ���ù�����
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
        /// ���򹥻���
        /// </summary>
        private void OnHitLookAtAttacker()
        {
            if (_currentAttacker == null) return;
            if(_animator.AnimationAtTag("Hit")||  _animator.AnimationAtTag("Parry") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f){
                transform.Look(_currentAttacker.position, 50f);
            }
        }

        /// <summary>
        /// �۳�����ֵ
        /// </summary>
        /// <param name="damage"></param>
        protected virtual void TakeDamage(float damage)
        {
            _characterHealthData.Damage(damage);
            GameEventManager.Instance.CallEvent<CharacterHealthBase>("UpdateHealthInfo", this);
        }

        /// <summary>
        /// ������ֵ
        /// </summary>
        /// <param name="damage"></param>
        protected virtual void TakeStrength(float damage)
        {
            _characterHealthData.DamageToStrength(damage);
            GameEventManager.Instance.CallEvent<CharacterHealthBase>("UpdateHealthInfo", this);
        }

        /// <summary>
        /// ��ɫ��������Ϊ,���Ŷ���ʲô��(��������д��)
        /// </summary>
        /// <param name="hitName">���˶���</param>
        /// <param name="parrName">�񵲶���</param>
        protected virtual void CharacterHitAction(float damage, string hitName, string parrName, DamagedType type)
        {

        }

        /// <summary>
        /// ������������
        /// </summary>
        protected virtual void PlayDeadAnimation()
        {
            if (!_animator.AnimationAtTag("FinishHit"))
            {
                _animator.Play("Dead1", 0, 0f);
            }
        } 

        /// <summary>
        /// ����������Ч
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
        /// �յ���ͨ����
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
        /// ��ʼ������������Ҫ׼���ñ������Ķ���
        /// </summary>
        /// <param name="hitName"></param>
        /// <param name="attacker"></param>
        public void BeginFinished(string hitName, Transform attacker)
        {
            SetAttacker(attacker);
            _animator.Play(hitName);
        }

        /// <summary>
        /// �����ڼ��ܵ����˺�
        /// </summary>
        /// <param name="damage">�˺�ֵ</param>
        /// <param name="type">�˺�����</param>
        /// <param name="isLastDamage">�Ƿ��Ǵ��������һ��</param>
        public void FinishDamage(float damage, DamagedType type,bool isLastDamage)
        {
            FinishDamageaccumulate += damage;
            //������Ч
            PlayHitClip(type);
            if (isLastDamage)
            {
                TakeDamage(damage);
                FinishDamageaccumulate = 0;
            }
        }
    }
}

