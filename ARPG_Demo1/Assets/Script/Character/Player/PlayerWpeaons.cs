using GGG.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWpeaons : MonoBehaviour
{
    [SerializeField] private Transform[] handsWeapon;       //双手持有武器
    [SerializeField] private Transform hipWeapon;           //后背武器
    private Animator _animator;
    private bool _isShow;                                   //判断当前手上的武器是否被显示

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
    /// 控制武器的显示
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
    /// 展示手上的武器
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
    /// 隐藏手上的武器
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
