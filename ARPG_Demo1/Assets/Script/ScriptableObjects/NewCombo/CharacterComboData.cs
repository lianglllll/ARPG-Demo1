using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_Combo_Data", menuName = "Character/Combo/NewComboData")]
public class CharacterComboData : ScriptableObject
{
    //һ����ʽ�ж�����������Դ�������˺�

    [SerializeField]
    private string _actionName;                                                         //��ʽ����Ҳ����Ҫ���ŵĶ�����
    [SerializeField]
    private List<ComboDamagedInfo> comboDamagedInfos = new List<ComboDamagedInfo>();    //��ʽ���˺���Ϣ
    [SerializeField]
    private float _actionColdTime;                                                      //��ʽ����ȴʱ�䣬�ν���һ�ζ�����Ҫ�ȴ���ʱ��
    [SerializeField]
    private float _comboPositionOffset;                                                 //����ι���ԽĿ��֮�䱣����Ѿ���
    [SerializeField]
    private CharacterComboData _nextComboData;                                          //��һ���νӱ����еĶ���
    [SerializeField]
    private CharacterComboData _childComboData;                                         //��������������˵ԭ���������һ�����У������Ҽ����ǾͲ�ִ��ǰ������У�����ִ���������������

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
/// �˺�����
/// </summary>
public enum DamagedType
{
    WEAPON,//�������͵��˺�
    PUNCH   //ȭ�����͵��˺�
}

[System.Serializable]
/// <summary>
/// �˺���Ϣ
/// </summary>
public class ComboDamagedInfo
{
    //�˺�����
    public DamagedType damagedType;
    //�˺�ֵ
    public float Damage;
    //���˶���
    public string HitName;
    //�񵲶���
    public string ParryName;
}
