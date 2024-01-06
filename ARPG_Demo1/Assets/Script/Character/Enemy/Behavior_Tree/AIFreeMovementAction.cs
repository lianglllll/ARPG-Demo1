using BehaviorDesigner.Runtime.Tasks;
using GGG.Tool;
using MyARPG.Combat;
using MyARPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFreeMovementAction : Action
{

    //1.在没有被指派攻击指令的时候，处于闲置自由移动
    //2.要么左右移动，要么前后移动
    //3.要么播放某些动画，比如idle（傻站着看着自己人被打，要么播放某些嘲讽动画）
    //4.在距离玩家过近时，需要让ai后退

    private EnemyMovementController _enemyMovementController;
    private EnemyCombatController _enemyCombatController;

    private int _actionIndex;           //动作索引，不同的值代表执行不同的动作
    private int _actionMaxIndex = 3;        //动作最大索引
    private int _lastActionIndex;       //上次执行的动作
    private float _actionTimer;             //动作执行时间
    private float _actionMinTimer = 1.5f;   //动作最小执行时间
    private float _actionMaxTimer = 3f;     //动作最大执行时间


    public override void OnAwake()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
        _enemyCombatController = GetComponent<EnemyCombatController>();
        _lastActionIndex = _actionIndex;
        _actionTimer = _actionMaxTimer;
    }


    public override TaskStatus OnUpdate()
    {
        //1.判断自身是否被指派攻击，如果是的话就说明当前节点就结束了
        if(_enemyCombatController.GetAttackCommand() == false)
        {
            //执行，处于当前节点的逻辑
            if (DistanceForTarget() > 8f)
            {//跟着敌人
                _enemyMovementController.SetAnimatorMovementValue(0f, 1f);
            }
            else if(DistanceForTarget() < 8.0f -0.1f && DistanceForTarget()>3f +0.1f)
            {//自由移动
                FreeMovement();
                UpdateFreeAction();
            }else
            {//后退
                _enemyMovementController.SetAnimatorMovementValue(1f, -1f);
            }
            return TaskStatus.Running;
        }
        else
        {
            //做一些退出逻辑
            return TaskStatus.Success;
        }   
    }


    private float DistanceForTarget() => 
        DevelopmentToos.DistanceForTarget(EnemyManager.Instance.GetMainPlayer(), _enemyMovementController.transform);


    /// <summary>
    /// 更新动作持续时间
    /// </summary>
    private void UpdateFreeAction()
    {
        if(_actionTimer > 0)
        {
            _actionTimer -= Time.deltaTime;
            if (_actionTimer <= 0)
            {
                _actionTimer = Random.Range(_actionMinTimer,_actionMaxTimer);
                UpdateActionIndex();
            }
        } 
    }

    /// <summary>
    /// 更新动作索引
    /// </summary>
    private void UpdateActionIndex()
    {
        _lastActionIndex = _actionIndex;
        _actionIndex = Random.Range(0, _actionMaxIndex);
        if(_actionIndex == _lastActionIndex)
        {
            _actionIndex = Random.Range(0, _actionMaxIndex);
        }
    }

    /// <summary>
    /// 执行动作
    /// </summary>
    private void FreeMovement()
    {
        switch (_actionIndex)
        {
            case 0://向左移动
                _enemyMovementController.SetAnimatorMovementValue(-1f, 0f);
                break;
            case 1://向右移动
                _enemyMovementController.SetAnimatorMovementValue(1f, 0f);
                break;
            case 2://傻站不动
                _enemyMovementController.SetAnimatorMovementValue(0f, 0f);
                break;
        }
    }





}
