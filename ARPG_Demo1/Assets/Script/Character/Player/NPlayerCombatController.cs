using GGG.Tool;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPlayerCombatController : NCharacterCombatBase
{
    private PlayerWpeaons _playerWpeaon;

    [SerializeField,Header("扩展招式")]
    protected CharacterComboData SwordComboData;                                    //剑法招式
    [SerializeField]
    protected CharacterComboData FinishComboData;                                   //处决招式    
    [SerializeField]
    protected CharacterComboData AssassinateComboData;                              //暗杀招式


    protected bool CanFinish
    {
        get
        {
            if (_currentEnemy == null) return false;
            return _currentEnemy.GetComponent<CharacterHealthBase>().CanFinish();
        }
    }


    protected override void Awake()
    {
        base.Awake();
        _playerWpeaon = GetComponent<PlayerWpeaons>();
    }

    protected override void Update()    
    {
        base.Update();
        ClearEnemy();
        PlayerAttackInput();
        CharacterFinishAttackInput();
        CharacterAssassinationInput();
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddEventListening<Transform>("EnemyDeath", EnemyDeathHandler);
    }
    private void OnDisable()
    {
        GameEventManager.Instance.RemoveEvent<Transform>("EnemyDeath", EnemyDeathHandler);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (transform.up * 0.7f), _detectionRange);
    }

    /// <summary>
    /// 清除目标
    /// </summary>
    private void ClearEnemy()
    {
        if (_currentEnemy == null) return;
        //角色移动的时候
        if (_animator.GetFloat(AnimationID.MovementID) > 0.7f && DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 3f)
        {
            _currentEnemy = null;
            GameEventManager.Instance.CallEvent("OnNotDetectEnemy");                        //触发清除检测角色事件
        }
    }

    /// <summary>
    /// 位置同步,找到和目标适合播放动画的位置
    /// </summary>
    protected override void MatchPosition()
    {
        base.MatchPosition();
        if (_currentEnemy == null) return;
        if (_animator == null) return;
        if (_animator.AnimationAtTag("Dead")) return;

        if (_animator.AnimationAtTag("Finish"))
        {
            transform.rotation = Quaternion.LookRotation(-_currentEnemy.forward);//看向目标，dir就是enemy的正后方
            RunningMatch(FinishComboData);
        }
        else if (_animator.AnimationAtTag("Assassination"))
        {
            transform.rotation = Quaternion.LookRotation(_currentEnemy.forward);//看向目标的背面，dir就是enemy的正前方
            RunningMatch(AssassinateComboData);
        }
    }


    /// <summary>
    /// 是否可以普通攻击
    /// </summary>
    /// <returns></returns>
    private bool CanBaseAttackInput()
    {
        //1._canAttackInput == false 不允许攻击
        //2. 如果角色正在挨揍，不允许攻击
        //3. 角色正在格挡，不允许攻击
        //4.角色正在处决，不允许攻击
        if (_applyAttackInput == false) return false;
        if (_animator.AnimationAtTag("Hit")) return false;
        if (_animator.AnimationAtTag("Parry")) return false;
        if (_animator.AnimationAtTag("Execute")) return false;
        if (_animator.AnimationAtTag("Finish")) return false;
        if (_animator.AnimationAtTag("Assassination")) return false;
        return true;
    }

    /// <summary>
    /// 玩家的普通攻击输入
    /// </summary>
    private void PlayerAttackInput()
    {
        if (!CanBaseAttackInput()) return;
        if (InputManager.Instance.LAttack)              //正常连招
        {   
            SetComboData(_currentComboData.NextComboData);
            ComboActionExecute();
        }
        else if (InputManager.Instance.RAttack)         //派生动作
        {
            if (!_currentComboData.HasChildComboData) return;
            SetComboData(_currentComboData.ChildComboData);
            ComboActionExecute();
        }
    }

    /// <summary>
    /// 是否可以处决,生命值低于某个值也可以处决
    /// </summary>
    /// <returns></returns>
    private bool CanSpecialAttack()
    {
        if (_currentEnemy == null) return false;
        if (_animator.AnimationAtTag("Parry")) return false;
        if (_animator.AnimationAtTag("Dash")) return false;
        if (_animator.AnimationAtTag("Hit")) return false;
        if (_animator.AnimationAtTag("Finish")) return false;            //Finish:处决
        if (_animator.AnimationAtTag("Assassination")) return false;
        if (!CanFinish) return false;
        return true;
    }

    /// <summary>
    /// 处决输入
    /// </summary>
    private void CharacterFinishAttackInput()
    {
        if (!CanSpecialAttack()) return;
        if (InputManager.Instance.Grab)
        {
            //敌人管理器取消全部攻击指令
            EnemyManager.Instance.StopAllActioveUnit();

            //1.去播放对应的处决动画
            _animator.Play(FinishComboData.ActionName);
            //2.告诉敌人他要被处决了，感觉播放动画
            if (_currentEnemy.transform.TryGetComponent(out IDamage damage))
            {
                damage.BeginFinished(FinishComboData.DamagedInfos[0].HitName, transform);
            }
            //3.呼叫事件中心,将相机注视敌人
            GameEventManager.Instance.CallEvent("SetMainCameraTarget", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);
            //4.处决完成后
            ResetComboData();
        }
    }

    /// <summary>
    /// 是否可以暗杀
    /// </summary>
    /// <returns></returns>
    private bool CanAssassination()
    {
        //1.当前没有目标
        if (_currentEnemy == null) return false;
        //2.距离太远
        if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return false;
        //3.角度太大
        if (Vector3.Angle(transform.forward, _currentEnemy.forward) > 30f) return false;
        //4.如果现在正在暗杀
        if (_animator.AnimationAtTag("Assassination")) return false;
        //5.现在在处决
        if (_animator.AnimationAtTag("Finish")) return false;
        return true;
    }

    /// <summary>
    /// 暗杀输入
    /// </summary>
    private void CharacterAssassinationInput()
    {

        if (InputManager.Instance.TakeOut)
        {
            //自动锁敌
            LockInRecentTargets(GetUnits());
            if (!CanAssassination()) return;

            //敌人管理器取消全部攻击指令
            EnemyManager.Instance.StopAllActioveUnit();

            //1.去播放对应的处决动画
            _animator.Play(AssassinateComboData.ActionName);
            //2.告诉敌人他要被处决了，感觉播放动画
            if (_currentEnemy.transform.TryGetComponent(out IDamage damage))
            {
                damage.BeginFinished(AssassinateComboData.DamagedInfos[0].HitName, transform);
            }
            //3.呼叫事件中心,将相机注视敌人
            GameEventManager.Instance.CallEvent("SetMainCameraTarget", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);
            //4.处决完成后
            ResetComboData();
        }
    }

    /// <summary>
    /// 处决动画触发伤害
    /// </summary>
    /// <param name="index"></param>
    protected void FinishATK(int index)
    {
        //1.我们要确保有目标
        if (_currentEnemy == null) return;
        _currentEnemy.GetComponent<CharacterHealthBase>().FinishDamage(FinishComboData.DamagedInfos[0].Damage, FinishComboData.DamagedInfos[0].damagedType, index==1);
    }

    /// <summary>
    /// 敌人死亡事件触发的回调
    /// </summary>
    /// <param name="enemy"></param>
    public void EnemyDeathHandler(Transform enemy)
    {
        if (_currentEnemy == enemy)
        {
            _currentEnemy = null;
        }
    }

    /// <summary>
    /// 物理交互
    /// 当其他物体碰到我们的碰撞器时，就给他一个向前的力
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody;
        if (hit.transform.TryGetComponent(out rigidbody))
        {
            rigidbody.AddForce(transform.forward * 20f, ForceMode.Force);
        }

    }
}
