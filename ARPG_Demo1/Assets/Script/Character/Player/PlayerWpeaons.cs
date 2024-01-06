using GGG.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWpeaons : MonoBehaviour
{
    [SerializeField] private Transform[] handsWeapon;       //˫�ֳ�������
    [SerializeField] private Transform hipWeapon;           //������
    private Animator _animator;
    private bool _isShow;                                   //�жϵ�ǰ���ϵ������Ƿ���ʾ

    public bool WeaponIsShow() => _isShow;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        HideWeapon();
    }

    private void Update()
    {
        ContrlShowWp();
    }

    /// <summary>
    /// ������������ʾ
    /// </summary>
    private void ContrlShowWp()
    {
        if (_animator.AnimationAtTag("Equip")) return;
        if (!_isShow)
        {
            if (InputManager.Instance.Equip)
            {
                _animator.Play("EquipWP");
            }
        }
        else
        {
            if (InputManager.Instance.Equip)
            {
                _animator.Play("UnEquipWP");
            }
        }
    }


    /// <summary>
    /// չʾ���ϵ�����
    /// </summary>
    public void ShowWeapon()
    {
        if (handsWeapon.Length == 0) return;
        foreach(var wp in handsWeapon)
        {
            wp.gameObject.SetActive(true);
        }
        _isShow = true;
        _animator.SetBool(AnimationID.ShowWPID, _isShow);
        hipWeapon.gameObject.SetActive(false);
    }


    /// <summary>
    /// �������ϵ�����
    /// </summary>
    public void HideWeapon()
    {
        if (handsWeapon.Length == 0) return;
        foreach (var wp in handsWeapon)
        {
            wp.gameObject.SetActive(false);
        }
        _isShow = false;
        _animator.SetBool(AnimationID.ShowWPID, _isShow);
        hipWeapon.gameObject.SetActive(true);
    }

}
