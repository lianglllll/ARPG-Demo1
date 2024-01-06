using BehaviorDesigner.Runtime.Tasks;
using GGG.Tool;
using MyARPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 条件节点
/// </summary>
public class AIAttckCommandCondition : Conditional
{
    private EnemyCombatController _enemyCombatController;

    public override void OnAwake()
    {
        _enemyCombatController = GetComponent<EnemyCombatController>();
    }

    public override TaskStatus OnUpdate()
    {
        return (_enemyCombatController.GetAttackCommand()) ? TaskStatus.Success : TaskStatus.Failure;
    }

}
