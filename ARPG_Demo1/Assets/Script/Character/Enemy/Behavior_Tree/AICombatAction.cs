using BehaviorDesigner.Runtime.Tasks;
using MyARPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatAction : Action
{
    private EnemyCombatController _enemyCombatController;

    public override void OnAwake()
    {
        _enemyCombatController = GetComponent<EnemyCombatController>();
    }


    public override TaskStatus OnUpdate()
    {
        if (_enemyCombatController.GetAttackCommand())
        {
            _enemyCombatController.AIBaseAttackInput();
            return TaskStatus.Running;
        }

        return TaskStatus.Success;
    }


}
