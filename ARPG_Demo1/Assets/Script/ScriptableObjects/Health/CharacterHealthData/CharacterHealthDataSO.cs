using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.HealthData
{
    [CreateAssetMenu(fileName = "CharacterHealthData", menuName = "Create/Character/HealthData/CharacterHealthData", order = 0)]
    public class CharacterHealthDataSO : ScriptableObject
    {

        [SerializeField] public CharacterHealthBaseDataSO _characterHealthBase;             //基础生命info


        //1.最大生命值
        private float _maxHP;
        //2.最大体力值
        private float _maxStrength;
        //3.当前生命值
        private float _currentHP;
        //4.当前体力值
        private float _currentStrength;
        //5.是否死亡
        public bool _isDeath =>(_currentHP <= 0);
        //6.当前体力是否充沛
        private bool _strengthIsFull;

        public float CurrentHP => _currentHP;
        public float CurrentStrength => _currentStrength;
        public float MaxHP => _maxHP;
        public float MaxStrength => _maxStrength;
        public bool StrengthFull => _strengthIsFull;


        /// <summary>
        /// 初始化
        /// </summary>
        public void InitCharacterHealthInfo()
        {
            _maxHP = _characterHealthBase.MaxHp;
            _maxStrength = _characterHealthBase.MaxStrength;
            _currentHP = _maxHP;
            _currentStrength = _maxStrength;
            _strengthIsFull = true;
        }

        /// <summary>
        /// 扣hp
        /// </summary>
        /// <param name="damage"></param>
        public void Damage(float damage)
        {
            //1.敌人现在正在攻击动作中，还没打到玩家，而被玩家打到了。此时敌人不允许格挡了
            //这时候不仅扣血而且扣体力值（惩罚）
            if (_strengthIsFull)
            {
                _currentStrength = Clmap(_currentStrength, damage, 0f, _maxStrength);
                if(_currentStrength <= 0)
                {
                    _strengthIsFull = false;
                }
            }
            _currentHP = Clmap(_currentHP, damage, 0f, _maxHP);

        }

        /// <summary>
        /// 扣strength
        /// </summary>
        /// <param name="damage"></param>
        public void DamageToStrength(float damage)
        {
            if (_strengthIsFull)
            {
                _currentStrength = Clmap(_currentStrength, damage, 0f, _maxStrength);
                if (_currentStrength <= 0)
                {
                    _strengthIsFull = false;
                }
            }
        }

        /// <summary>
        /// 添加hp
        /// </summary>
        /// <param name="hp"></param>
        public void AddHP(float hp)
        {
            _currentHP = Clmap(_currentHP, hp, 0f, _maxHP, true);
        }

        /// <summary>
        /// 添加strength
        /// </summary>
        /// <param name="strength"></param>
        public void AddStrength(float strength)
        {
            _currentStrength = Clmap(_currentStrength, strength, 0f, _maxHP, true);
            if(_currentStrength >= _maxStrength)
            {
                _strengthIsFull = true;
            }
        }

        private float Clmap(float value,float offset,float min,float max,bool add = false)
        {
            return Mathf.Clamp((add) ? value + offset : value - offset, min, max);
        }

    }

}

