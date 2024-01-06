using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace MyARPG.Assets
{


    [CreateAssetMenu(fileName ="Sound",menuName ="Create/Assets/Sound",order =0)]
    public class AssetsSoundSO : ScriptableObject
    {
        [System.Serializable]
        private class Sounds
        {
            public SoundType SoundType;
            public AudioClip[] AudioClips;

        }

        [SerializeField] private List<Sounds> _configSound = new List<Sounds>();



        public AudioClip GetAudioClip(SoundType type)
        {
            if (_configSound.Count == 0) return null;
            switch (type)
            {
                case SoundType.ATK:
                    return _configSound[0].AudioClips[Random.Range(0,_configSound[0].AudioClips.Length)];
                case SoundType.HIT:
                    return _configSound[1].AudioClips[Random.Range(0, _configSound[1].AudioClips.Length)];
                case SoundType.BLOCK:
                    return _configSound[2].AudioClips[Random.Range(0, _configSound[2].AudioClips.Length)];
                case SoundType.FOOT:
                    return _configSound[3].AudioClips[Random.Range(0, _configSound[3].AudioClips.Length)];
                case SoundType.Sword:
                    return _configSound[4].AudioClips[Random.Range(0, _configSound[4].AudioClips.Length)];
                case SoundType.SwordHit:
                    return _configSound[5].AudioClips[Random.Range(0, _configSound[5].AudioClips.Length)];
            }
            return null;
        }


    }

}
