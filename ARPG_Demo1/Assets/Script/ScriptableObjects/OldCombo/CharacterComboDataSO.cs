using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.ComboData
{
    /// <summary>
    /// 招式的类型
    /// </summary>
    public enum ComboDataType
    {
        PUNCH,
        TICK,
        WEAPON
    }

    /// <summary>
    /// 单个招式的数据:野马分鬃，单个招式可以又多个动作组成，所有可以又多段伤害
    /// </summary>
    [CreateAssetMenu(fileName = "ComboData", menuName = "Create/Character/ComboData", order = 0)]
    public class CharacterComboDataSO : ScriptableObject
    {
        [SerializeField] private string _comboName;             //招式名称（某个动画片段的名字），要播放某个动画，只需要去取得某个动作的动作名就可以了
        [SerializeField] private string[] _comboHitName;        //有多个动画来形成这个招式
        [SerializeField] private string[] _comboParryName;      //格挡的动画
        [SerializeField] private float _damage;                 //伤害
        [SerializeField] private float _coldTime;               //衔接下一段攻击的时间间隔(其实是当前招式的播放时间)
        [SerializeField] private float _comboPositionOffset;    //让这段攻击越目标之间保存最佳距离
        [SerializeField] private ComboDataType _comboDataType;

        public string ComboName => _comboName;
        public string[] ComboHitName => _comboHitName;
        public string[] ComboParryName => _comboParryName;
        public float Damage => _damage;
        public float ColdTime => _coldTime;
        public float ComboPositionOffset => _comboPositionOffset;

        /// <summary>
        /// 获取当前动作最大的受伤数
        /// </summary>
        /// <returns></returns>
        public int GetHitAndParryNameMaxCount() => _comboHitName.Length;


        public ComboDataType GetCombDataType() => _comboDataType;
    }

}

