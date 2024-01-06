using GGG.Tool.Singleton;
using MyARPG.Combat;
using MyARPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ���˹����������˸����
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{
    private Transform _mainPlayer;
    [SerializeField]private List<GameObject> _allEnemy = new List<GameObject>();
    [SerializeField]private List<GameObject> _allActiveEnemy = new List<GameObject>();

    private WaitForSeconds _waitTime;
    private bool _closeAttackCommandCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _mainPlayer = GameObject.FindWithTag("Player").transform;
        _waitTime = new WaitForSeconds(10);
    }

    private void Start()
    {
        InitActiveEnemy();
        StartAttackCommandCoroutine();
    }


    private void OnDestroy()
    {
    }


    public Transform GetMainPlayer() => _mainPlayer;

    public void AddEnemyUnit(GameObject enemy)
    {
        if (!_allEnemy.Contains(enemy))
        {
            _allEnemy.Add(enemy);
        }
    }

    public void RemoveEnemyUnit(GameObject enemy)
    {
        if (_allEnemy.Contains(enemy))
        {
            _allEnemy.Remove(enemy);
        }
    }

    public void AddActiveEnemyUnit(GameObject enemy)
    {
        if (!_allActiveEnemy.Contains(enemy))
        {
            _allActiveEnemy.Add(enemy);
            EnemyMovementController enemyMovementController;
            if (enemy.TryGetComponent(out enemyMovementController))
            {
                enemyMovementController.EnableCharacterController(true);
            }
        }

    }

    public void RemoveActiveEnemyUnit(GameObject enemy)
    {
        if (_allActiveEnemy.Contains(enemy))
        {
            EnemyMovementController enemyMovementController;
            if (enemy.TryGetComponent(out enemyMovementController))
            {
                enemyMovementController.EnableCharacterController(false);
            }
            _allActiveEnemy.Remove(enemy);
        }

    }


    /// <summary>
    /// Э��,��ai�ַ�����ָ��
    /// </summary>
    /// <returns></returns>
    IEnumerator EnableEnemyUnitAttackCommand()
    {
        if (_allActiveEnemy == null) yield break;//�ر�Э��
        if (_allActiveEnemy.Count == 0) yield break;

        while (_allActiveEnemy.Count > 0)
        {
            if (_closeAttackCommandCoroutine) yield break;
            var index = Random.Range(0, _allActiveEnemy.Count);
            if (index < _allActiveEnemy.Count)
            {
                GameObject obj = _allActiveEnemy[index];
                EnemyCombatController enemyCombatController;
                if (obj.TryGetComponent(out enemyCombatController))
                {
                    enemyCombatController.SetAttackCommand(true, _mainPlayer);
                }
            }

            yield return _waitTime;
        }

    }


    public void StopAllActioveUnit()
    {
        foreach(var e in _allActiveEnemy)
        { 
            EnemyCombatController enemyCombatController; 
            if (e.TryGetComponent(out enemyCombatController))
            {
                enemyCombatController.StopAllAction();
            }
        }
    }

    private void InitActiveEnemy()
    {
        foreach (var e in _allEnemy)
        {
            if (e.activeSelf)
            {
                EnemyMovementController enemyMovementController;
                if(e.TryGetComponent(out enemyMovementController))
                {
                    enemyMovementController.EnableCharacterController(true);
                }
                _allActiveEnemy.Add(e);
                
            }
        }
    }


    public void CloseAttackCommandCoroutine()
    {
        _closeAttackCommandCoroutine = true;
        StopCoroutine(EnableEnemyUnitAttackCommand());
    }
    public void StartAttackCommandCoroutine()
    {
        if (_allActiveEnemy.Count > 0)
        {
            _closeAttackCommandCoroutine = false;
        }
        StartCoroutine(EnableEnemyUnitAttackCommand());
    }


}
