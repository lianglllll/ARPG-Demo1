using MyARPG.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum SoundType
{
    ATK,    //����
    HIT,    //����
    BLOCK,  //��
    FOOT,   //�Ų���
    Sword,  //��
    SwordHit,//������
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
        //����������ʱ����������
        //����������Ὺʼ��ʱ��0.3������������
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
