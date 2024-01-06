using MyARPG.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum SoundType
{
    ATK,    //攻击
    HIT,    //挨揍
    BLOCK,  //格挡
    FOOT,   //脚步声
    Sword,  //剑
    SwordHit,//被剑砍
}

public class PoolItemSound : PoolItemBase
{
    private AudioSource _audioSource;
    [SerializeField] private SoundType _soundType;
    [SerializeField] private AssetsSoundSO _soundAssets;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public override void Spawn()
    {
        //当自身被激活时，播放声音
        //播放声音后会开始计时，0.3秒后自身会隐藏
        PlaySound();
    }

    public override void Recycle()
    {
    }


    private void PlaySound()
    {
        _audioSource.clip = _soundAssets.GetAudioClip(_soundType);
        _audioSource.Play();
        StartRecycle();
    }

    private void StartRecycle()
    {
        GameTimerManager.Instance.TryUseOneTimer(0.3f, DisableSelf);
    }

    private void DisableSelf()
    {
        _audioSource.Stop();
        this.gameObject.SetActive(false);
    }

}
