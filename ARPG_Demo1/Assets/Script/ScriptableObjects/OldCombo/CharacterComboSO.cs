using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.ComboData
{

    /// <summary>
    /// 连招表。比如说太极，里面有很多招式：野马分鬃，白鹤亮翅,搂膝拗步
    /// </summary>
    [CreateAssetMenu(fileName = "Combo", menuName = "Create/Character/Combo", order = 0)]
    public class CharacterComboSO : ScriptableObject
    {
        [SerializeField] private List<CharacterComboDataSO> _allComboData = new List<CharacterComboDataSO>();


        /// <summary>
        /// 尝试获取连招中某个招式动作
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string TryGetOneComboAction(int index) 
        {
            if (_allComboData.Count == 0) return null;
            return _allComboData[index].ComboName;
        }

        /// <summary>
        /// 获取对应的受伤动画
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
        /// 获取对应的格挡动画
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
        /// 获取招式的伤害值
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
        /// 获取衔接下一段攻击的时间间隔
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


        //连招一共有几个招式
        public int TryGetComboMaxCount() => _allComboData.Count;


        public ComboDataType GetComboDataType(int index)
        {
            if (_allComboData.Count == 0) return 0f;
            return _allComboData[index].GetCombDataType();
        }


    }
}


