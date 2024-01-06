using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warrior {

    /// <summary>
    /// 音频类型
    /// </summary>
    public enum AudioClipType
    {
        FOOT,PUNCH,PUNCHHIT,PUNCHBLOCK,SWORDATTACK,HEAVYSWORDATTACK,SWORDHIT,SWORDBLOCK
    }

    [System.Serializable]
    public class AudioClipAssetsConfig
    {
        public AudioClipType audioClipType;
        public AudioClip[] clips;
    }

    [CreateAssetMenu(menuName ="GameData/Assets/AudioAssets",fileName ="NewAudioAssets",order = 0)]
    public class GameAudioAssets : MonoBehaviour
    {
        [SerializeField] private List<AudioClipAssetsConfig> _allAudioClipAssets = new List<AudioClipAssetsConfig>();

        /// <summary>
        /// 获取音频资源
        /// </summary>
        /// <param name="clipType"></param>
        /// <returns></returns>
        public AudioClip TryGetAudioClipAssets(AudioClipType clipType)
        {
            if (_allAudioClipAssets.Count == 0) return null;
            foreach(var e in _allAudioClipAssets)
            {
                if(e.audioClipType == clipType)
                {
                    return e.clips[Random.Range(0, e.clips.Length)];
                }
            }
            return null;
        }
    }
}





