using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.ComboData
{

    /// <summary>
    /// ���б�����˵̫���������кܶ���ʽ��Ұ����ף��׺�����,§ϥ�ֲ�
    /// </summary>
    [CreateAssetMenu(fileName = "Combo", menuName = "Create/Character/Combo", order = 0)]
    public class CharacterComboSO : ScriptableObject
    {
        [SerializeField] private List<CharacterComboDataSO> _allComboData = new List<CharacterComboDataSO>();


        /// <summary>
        /// ���Ի�ȡ������ĳ����ʽ����
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string TryGetOneComboAction(int index) 
        {
            if (_allComboData.Count == 0) return null;
            return _allComboData[index].ComboName;
        }

        /// <summary>
        /// ��ȡ��Ӧ�����˶���
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hitIndex"></param>
        /// <returns></returns>
        public string TryGetOneHitName(int index,int hitIndex)
        {
            if (_allComboData.Count == 0) return null;
            if (_allComboData[index].GetHitAndParryNameMaxCount() == 0) return null;
            return _allComboData[index].ComboHitName[hitIndex];
        }

        /// <summary>
        /// ��ȡ��Ӧ�ĸ񵲶���
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hitIndex"></param>
        /// <returns></returns>
        public string TryGetOneParryName(int index, int hitIndex)
        {
            if (_allComboData.Count == 0) return null;
            if (_allComboData[index].GetHitAndParryNameMaxCount() == 0) return null;
            return _allComboData[index].ComboParryName[hitIndex];
        }

        /// <summary>
        /// ��ȡ��ʽ���˺�ֵ
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float TryGetComboDamage(int index)
        {
            if (_allComboData.Count == 0) return 0;
            if (index < 0 || index >= _allComboData.Count) return 0;
            return _allComboData[index].Damage;
        }

        /// <summary>
        /// ��ȡ�ν���һ�ι�����ʱ����
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float TryGetComboColdTime(int index)
        {
            if (_allComboData.Count == 0) return 0;
            return _allComboData[index].ColdTime;
        }

        public float TryGetComboPositionOffset(int index)
        {
            if (_allComboData.Count == 0) return 0;
            if (index < 0 || index >= _allComboData.Count) return 0;
            return _allComboData[index].ComboPositionOffset;
        }


        public int TryGetHitOrParryMaxCount(int index)
        {
            return _allComboData[index].GetHitAndParryNameMaxCount();
        }


        //����һ���м�����ʽ
        public int TryGetComboMaxCount() => _allComboData.Count;


        public ComboDataType GetComboDataType(int index)
        {
            if (_allComboData.Count == 0) return 0f;
            return _allComboData[index].GetCombDataType();
        }


    }
}


