using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.HealthData
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Create/Character/HealthData/BaseData", order = 0)]
    public class CharacterHealthBaseDataSO : ScriptableObject
    {
        //1.����ÿһ�ֵ��˵ĳ�ʼ����ֵ��Ϣ
        //�����ж��ֵ��ˣ�����˱Ƶ��˳�ʼ����ֵ�ϵͣ�ǿ׳���˳�ʼ����ֵ�Ƚϸߣ�����Ҳ���ܸ�һ��
        [SerializeField] private float _maxHP;          //����ʼ����ֵ
        [SerializeField] private float _maxStrength;    //����ʼ����ֵ����Ϊ0���Ը�

        public float MaxHp => _maxHP;
        public float MaxStrength => _maxStrength;











    }

}

