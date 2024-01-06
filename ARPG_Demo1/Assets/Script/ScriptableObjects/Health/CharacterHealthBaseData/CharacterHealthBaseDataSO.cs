using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.HealthData
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Create/Character/HealthData/BaseData", order = 0)]
    public class CharacterHealthBaseDataSO : ScriptableObject
    {
        //1.处理每一种敌人的初始生命值信息
        //可能有多种敌人，比如菜逼敌人初始生命值较低，强壮敌人初始生命值比较高，抗性也可能高一点
        [SerializeField] private float _maxHP;          //最大初始生命值
        [SerializeField] private float _maxStrength;    //最大初始体力值，不为0可以格挡

        public float MaxHp => _maxHP;
        public float MaxStrength => _maxStrength;











    }

}

