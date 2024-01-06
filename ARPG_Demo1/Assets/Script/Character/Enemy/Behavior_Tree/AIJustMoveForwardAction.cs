using BehaviorDesigner.Runtime.Tasks;
using GGG.Tool;
using MyARPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ai¿¿½üÍæ¼Ò
/// </summary>
public class AIJustMoveForwardAction : Action
{
    private EnemyMovementController _enemyMovementController;
    public override void OnAwake()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
    }
    public override TaskStatus OnUpdate()
    {
        if(DevelopmentToos.DistanceForTarget(EnemyManager.Instance.GetMainPlayer(),_enemyMovementController.transform) > 1.5f)
        {
            _enemyMovementController.SetApplyMovement(true);
            _enemyMovementController.SetAnimatorMovementValue(0f, 1f);
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

}
