using BehaviorDesigner.Runtime.Tasks;
using GGG.Tool;
using MyARPG.Combat;
using MyARPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFreeMovementAction : Action
{

    //1.��û�б�ָ�ɹ���ָ���ʱ�򣬴������������ƶ�
    //2.Ҫô�����ƶ���Ҫôǰ���ƶ�
    //3.Ҫô����ĳЩ����������idle��ɵվ�ſ����Լ��˱���Ҫô����ĳЩ��������
    //4.�ھ�����ҹ���ʱ����Ҫ��ai����

    private EnemyMovementController _enemyMovementController;
    private EnemyCombatController _enemyCombatController;

    private int _actionIndex;           //������������ͬ��ֵ����ִ�в�ͬ�Ķ���
    private int _actionMaxIndex = 3;        //�����������
    private int _lastActionIndex;       //�ϴ�ִ�еĶ���
    private float _actionTimer;             //����ִ��ʱ��
    private float _actionMinTimer = 1.5f;   //������Сִ��ʱ��
    private float _actionMaxTimer = 3f;     //�������ִ��ʱ��


    public override void OnAwake()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
        _enemyCombatController = GetComponent<EnemyCombatController>();
        _lastActionIndex = _actionIndex;
        _actionTimer = _actionMaxTimer;
    }


    public override TaskStatus OnUpdate()
    {
        //1.�ж������Ƿ�ָ�ɹ���������ǵĻ���˵����ǰ�ڵ�ͽ�����
        if(_enemyCombatController.GetAttackCommand() == false)
        {
            //ִ�У����ڵ�ǰ�ڵ���߼�
            if (DistanceForTarget() > 8f)
            {//���ŵ���
                _enemyMovementController.SetAnimatorMovementValue(0f, 1f);
            }
            else if(DistanceForTarget() < 8.0f -0.1f && DistanceForTarget()>3f +0.1f)
            {//�����ƶ�
                FreeMovement();
                UpdateFreeAction();
            }else
            {//����
                _enemyMovementController.SetAnimatorMovementValue(1f, -1f);
            }
            return TaskStatus.Running;
        }
        else
        {
            //��һЩ�˳��߼�
            return TaskStatus.Success;
        }   
    }


    private float DistanceForTarget() => 
        DevelopmentToos.DistanceForTarget(EnemyManager.Instance.GetMainPlayer(), _enemyMovementController.transform);


    /// <summary>
    /// ���¶�������ʱ��
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
    /// ���¶�������
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
    /// ִ�ж���
    /// </summary>
    private void FreeMovement()
    {
        switch (_actionIndex)
        {
            case 0://�����ƶ�
                _enemyMovementController.SetAnimatorMovementValue(-1f, 0f);
                break;
            case 1://�����ƶ�
                _enemyMovementController.SetAnimatorMovementValue(1f, 0f);
                break;
            case 2://ɵվ����
                _enemyMovementController.SetAnimatorMovementValue(0f, 0f);
                break;
        }
    }





}
