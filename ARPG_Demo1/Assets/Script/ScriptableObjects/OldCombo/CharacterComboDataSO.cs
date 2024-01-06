using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.ComboData
{
    /// <summary>
    /// ��ʽ������
    /// </summary>
    public enum ComboDataType
    {
        PUNCH,
        TICK,
        WEAPON
    }

    /// <summary>
    /// ������ʽ������:Ұ����ף�������ʽ�����ֶ��������ɣ����п����ֶ���˺�
    /// </summary>
    [CreateAssetMenu(fileName = "ComboData", menuName = "Create/Character/ComboData", order = 0)]
    public class CharacterComboDataSO : ScriptableObject
    {
        [SerializeField] private string _comboName;             //��ʽ���ƣ�ĳ������Ƭ�ε����֣���Ҫ����ĳ��������ֻ��Ҫȥȡ��ĳ�������Ķ������Ϳ�����
        [SerializeField] private string[] _comboHitName;        //�ж���������γ������ʽ
        [SerializeField] private string[] _comboParryName;      //�񵲵Ķ���
        [SerializeField] private float _damage;                 //�˺�
        [SerializeField] private float _coldTime;               //�ν���һ�ι�����ʱ����(��ʵ�ǵ�ǰ��ʽ�Ĳ���ʱ��)
        [SerializeField] private float _comboPositionOffset;    //����ι���ԽĿ��֮�䱣����Ѿ���
        [SerializeField] private ComboDataType _comboDataType;

        public string ComboName => _comboName;
        public string[] ComboHitName => _comboHitName;
        public string[] ComboParryName => _comboParryName;
        public float Damage => _damage;
        public float ColdTime => _coldTime;
        public float ComboPositionOffset => _comboPositionOffset;

        /// <summary>
        /// ��ȡ��ǰ��������������
        /// </summary>
        /// <returns></returns>
        public int GetHitAndParryNameMaxCount() => _comboHitName.Length;


        public ComboDataType GetCombDataType() => _comboDataType;
    }

}

