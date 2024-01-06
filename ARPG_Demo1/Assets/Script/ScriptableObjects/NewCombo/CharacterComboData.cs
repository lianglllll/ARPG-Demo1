using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_Combo_Data", menuName = "Character/Combo/NewComboData")]
public class CharacterComboData : ScriptableObject
{
    //一个招式有多个动作，可以触发多个伤害

    [SerializeField]
    private string _actionName;                                                         //招式名，也就是要播放的动画名
    [SerializeField]
    private List<ComboDamagedInfo> comboDamagedInfos = new List<ComboDamagedInfo>();    //招式的伤害信息
    [SerializeField]
    private float _actionColdTime;                                                      //招式的冷却时间，衔接下一段动作需要等待的时间
    [SerializeField]
    private float _comboPositionOffset;                                                 //让这段攻击越目标之间保存最佳距离
    [SerializeField]
    private CharacterComboData _nextComboData;                                          //下一个衔接本连招的动作
    [SerializeField]
    private CharacterComboData _childComboData;                                         //派生动作，比如说原本按左键是一套连招，按了右键就是就不执行前面的连招，而是执行这个派生动作了

    public string ActionName
    {
        get
        {
            return _actionName;
        }
    }
    public List<ComboDamagedInfo> DamagedInfos
    {
        get
        {
            return comboDamagedInfos;
        }
    }
    public float ActionColdTime
    {
        get
        {
            return _actionColdTime;
        }
    }
    public CharacterComboData NextComboData
    {
        get
        {
            return _nextComboData;
        }
    }
    public CharacterComboData ChildComboData
    {
        get
        {
            return _childComboData;
        }
    }
    public bool HasChildComboData
    {
        get
        {
            return _childComboData != null;
        }
    }
    public float GetPositionOffset
    {
        get
        {
            return _comboPositionOffset;
        }
    }

}

/// <summary>
/// 伤害类型
/// </summary>
public enum DamagedType
{
    WEAPON,//武器类型的伤害
    PUNCH   //拳脚类型的伤害
}

[System.Serializable]
/// <summary>
/// 伤害信息
/// </summary>
public class ComboDamagedInfo
{
    //伤害类型
    public DamagedType damagedType;
    //伤害值
    public float Damage;
    //受伤动画
    public string HitName;
    //格挡动画
    public string ParryName;
}
