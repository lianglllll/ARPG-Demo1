using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class CharacterHitFXHelper : MonoBehaviour, IFX
{
    private ParticleSystem _particle;
    private void Awake()
    {
        _particle = transform.Find("HitFX").GetComponent<ParticleSystem>();
    }



    /// <summary>
    /// ����������Ч
    /// </summary>
    public void Play()
    {
        _particle.Play();
    }
}
