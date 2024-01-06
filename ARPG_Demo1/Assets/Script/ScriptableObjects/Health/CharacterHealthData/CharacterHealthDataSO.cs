using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.HealthData
{
    [CreateAssetMenu(fileName = "CharacterHealthData", menuName = "Create/Character/HealthData/CharacterHealthData", order = 0)]
    public class CharacterHealthDataSO : ScriptableObject
    {

        [SerializeField] public CharacterHealthBaseDataSO _characterHealthBase;             //��������info


        //1.�������ֵ
        private float _maxHP;
        //2.�������ֵ
        private float _maxStrength;
        //3.��ǰ����ֵ
        private float _currentHP;
        //4.��ǰ����ֵ
        private float _currentStrength;
        //5.�Ƿ�����
        public bool _isDeath =>(_currentHP <= 0);
        //6.��ǰ�����Ƿ����
        private bool _strengthIsFull;

        public float CurrentHP => _currentHP;
        public float CurrentStrength => _currentStrength;
        public float MaxHP => _maxHP;
        public float MaxStrength => _maxStrength;
        public bool StrengthFull => _strengthIsFull;


        /// <summary>
        /// ��ʼ��
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
        /// ��hp
        /// </summary>
        /// <param name="damage"></param>
        public void Damage(float damage)
        {
            //1.�����������ڹ��������У���û����ң�������Ҵ��ˡ���ʱ���˲��������
            //��ʱ�򲻽���Ѫ���ҿ�����ֵ���ͷ���
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
        /// ��strength
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
        /// ���hp
        /// </summary>
        /// <param name="hp"></param>
        public void AddHP(float hp)
        {
            _currentHP = Clmap(_currentHP, hp, 0f, _maxHP, true);
        }

        /// <summary>
        /// ���strength
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

