using GGG.Tool;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NCharacterCombatBase : MonoBehaviour
{
    [Header("动画参数")]
    protected Animator _animator;
    [SerializeField] protected float _animationCrossNormalTime;                     //动画转换的混合时间

    [SerializeField]
    protected CharacterComboData _currentComboData;
    [SerializeField]
    protected CharacterComboData DefaultComboData;                                  //默认招式，招式第一个动作为空
    private float _attackColdTime;
    protected bool _applyAttackInput;

    [Header("敌人范围检测")]
    [SerializeField] protected float _detectionRange;
    [SerializeField] private LayerMask _enemyLayer;

    [SerializeField,Header("敌人信息")]
    protected Transform _currentEnemy;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        _attackColdTime = 0f;
        _applyAttackInput = true;
        _currentComboData = DefaultComboData;
    }

    protected virtual void Update()
    {
        MatchPosition();
        LookTargetOnAttack();
        OnEndAttack();
    }

    #region 位置同步,找到和目标适合播放动画的位置

    protected virtual void MatchPosition()
    {
        if (_currentEnemy == null) return;
        if (_animator == null) return;
        if (_animator.AnimationAtTag("Dead")) return;

        if (_animator.AnimationAtTag("Attack"))
        {
            var timer = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (timer > 0.35f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return;
            RunningMatch(_currentComboData, 0f, 0.45f);
        }
    }

    /// <summary>
    /// 位置匹配
    /// </summary>
    /// <param name="combo"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    protected void RunningMatch(CharacterComboData combo, float startTime = 0f, float endTime = 0.1f)
    {
        if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))//当前不在匹配同时不在过度状态
        {
            _animator.MatchTarget(_currentEnemy.position + (-transform.forward * combo.GetPositionOffset), Quaternion.identity,
                AvatarTarget.Body, new MatchTargetWeightMask(Vector3.one, 0f), startTime, endTime);
        }

    }

    #endregion

    /// <summary>
    /// 攻击时看着目标
    protected void LookTargetOnAttack()
    {
        if (_currentEnemy == null) return;
        if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 5f) return;
        //攻击的时候，看向敌人
        if (_animator.AnimationAtTag("Attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            transform.Look(_currentEnemy.position, 1000f);
        }
    }

    /// <summary>
    /// 设置当前的招式
    /// </summary>
    /// <param name="characterComboData"></param>
    protected void SetComboData(CharacterComboData characterComboData)
    {
        if (characterComboData == null) return;
        _currentComboData = characterComboData;
    }

    /// <summary>
    /// 攻击动作执行,播放动画
    /// </summary>
    protected void ComboActionExecute()
    {
        _attackColdTime = _currentComboData.ActionColdTime;
        _animator.CrossFade(_currentComboData.ActionName, 0.025f, 0, 0f);
        _applyAttackInput = false;
        GameTimerManager.Instance.TryUseOneTimer(_attackColdTime, ResetAttackInput);
    }

    /// <summary>
    /// 重置攻击输入
    /// </summary>
    protected void ResetAttackInput()
    {
        _applyAttackInput = true;
    }

    /// <summary>
    /// 动画攻击事件触发回调
    /// </summary>
    /// <param name="index">动画传的index</param>
    private void ATK(int index)
    {
        TrggerDamaged(index);
        PlayATKSound(index);
    }

    /// <summary>
    /// 获取周围的敌人
    /// </summary>
    /// <returns></returns>
    protected Collider[] GetUnits()
    {
        return Physics.OverlapSphere(transform.position + (transform.up) * 0.7f, _detectionRange, _enemyLayer, QueryTriggerInteraction.Ignore);//没有collider的检测不到
    }

    /// <summary>
    /// 锁定最近的目标
    /// </summary>
    /// <returns></returns>
    protected int LockInRecentTargets(Collider[] enemys)
    {
        if (enemys == null) return -1;
        if (enemys.Length == 0) return -1;
        if (_currentEnemy == null)
        {
            _currentEnemy = enemys[0].transform;
        }else if (_currentEnemy != enemys[0].transform)
        {
            _currentEnemy = enemys[0].transform;
        }
        else
        {
            _currentEnemy = enemys[0].transform;
        }
        GameEventManager.Instance.CallEvent<Transform>("OnDetectEnemy", _currentEnemy);         //触发检测到敌人的事件
        return 0;
    }

    /// <summary>
    /// 触发伤害，告诉敌人它被揍了
    /// 1.判断是否附加有敌人,GetUnits()获取附加的敌人
    /// 2.判断敌人的角度是否符合触发伤害
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void TrggerDamaged(int index)
    {
        Collider[] enemys = GetUnits();
        if(LockInRecentTargets(enemys) < 0)
        {
            return;
        }
        //防止越界
        index = Mathf.Min(index, _currentComboData.DamagedInfos.Count-1);
        //判断敌人的角度
        foreach(var e in enemys)
        {
            //如果处于0.85f说明敌人处于玩家的前方，并且在允许触发伤害的角度内
            if (Vector3.Dot(DevelopmentToos.DirectionForTarget(e.transform, transform), transform.forward) > 0.85f) continue;
            if (DevelopmentToos.DistanceForTarget(e.transform, transform) > 2f) continue;
            if(e.transform.TryGetComponent(out IDamage damage))
            {
                damage.CharacterNormalDamage(_currentComboData.DamagedInfos[index].HitName, _currentComboData.DamagedInfos[index].ParryName,
                    _currentComboData.DamagedInfos[index].Damage, transform, _currentComboData.DamagedInfos[index].damagedType);
            }

        }

    }

    /// <summary>
    /// 根据攻击类型播放攻击音效
    /// </summary>
    /// <param name="type"></param>
    public void PlayATKSound(int index)
    {
        DamagedType type = _currentComboData.DamagedInfos[index].damagedType;
        switch (type)
        {
            case DamagedType.PUNCH:
                GamePoolManager.Instance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
                break;
            case DamagedType.WEAPON:
                GamePoolManager.Instance.TryGetPoolItem("SwordSound", transform.position, Quaternion.identity);
                break;
        }
    }
    //动画用
    public void PlaySound(string str)
    {
        GamePoolManager.Instance.TryGetPoolItem(str, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 攻击动作结束后，招式重置为第一招
    /// </summary>
    protected void OnEndAttack()
    {
        //动作在移动&&可以接收输入的话
        //说明我在上一段攻击结束之后，我已经不继续攻击了，我要开始动了
        if (_animator.AnimationAtTag("Motion") && _applyAttackInput)
        {
            ResetComboData();
        }

    }

    /// <summary>
    /// 重置招式
    /// </summary>
    protected virtual void ResetComboData()
    {
        //根据是否佩戴武器来决定使用哪个招式
        _currentComboData = DefaultComboData;
    }

}
