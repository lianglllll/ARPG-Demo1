using BehaviorDesigner.Runtime.Tasks;
using GGG.Tool;
using MyARPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackDistanceCondition : Conditional
{


    private EnemyCombatController _enemyCombatController;
    [SerializeField] private float _attackDistance;

    public override void OnAwake()
    {
        _enemyCombatController = GetComponent<EnemyCombatController>();
    }

    public override TaskStatus OnUpdate()
    {
        return (DevelopmentToos.DistanceForTarget(EnemyManager.Instance.GetMainPlayer(), _enemyCombatController.transform) > _attackDistance) ? TaskStatus.Success : TaskStatus.Failure;
    }
}
