using GGG.Tool.Singleton;
using MyARPG.Health;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanel : Singleton<CombatPanel>
{
    private Transform _mainPlayer;
    private Transform RevivePoint;
    private HealthInfoBox MyHealthInfoBox;
    private HealthInfoBox CurrentEnemyHealthInfoBox;

    
    protected override void Awake()
    {
        base.Awake();
        _mainPlayer = GameObject.FindWithTag("Player").transform;
        RevivePoint = GameObject.Find("RevivePoint").transform;
        MyHealthInfoBox = transform.Find("MyHealthInfoBox").GetComponent<HealthInfoBox>();
        CurrentEnemyHealthInfoBox = transform.Find("CurrentEnemyHealthInfoBox").GetComponent<HealthInfoBox>();
    }
    private void Start()
    {
        //������ʾ�Լ�������Ϣ��ui
        MyHealthInfoBox.SetHealthInfo(_mainPlayer.GetComponent<PlayerHealthController>());
        SetEnemyHealthInfo(null);


        //�������������¼�
        GameEventManager.Instance.AddEventListening<Transform>("EnemyDeath", EnemyDeathHandler);
        //����player��⵽����
        GameEventManager.Instance.AddEventListening<Transform>("OnDetectEnemy", OnDetectEnemyHandler);
        GameEventManager.Instance.AddEventListening("OnNotDetectEnemy", OnNotDetectEnemyHandler);

    }



    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveEvent<Transform>("EnemyDeath", EnemyDeathHandler);
        GameEventManager.Instance.RemoveEvent<Transform>("OnDetectEnemy", OnDetectEnemyHandler);
        GameEventManager.Instance.RemoveEvent("OnNotDetectEnemy", OnNotDetectEnemyHandler);
    }


    /// <summary>
    /// player��⵽���˵Ļص�
    /// </summary>
    /// <param name="obj"></param>
    private void OnDetectEnemyHandler(Transform obj)
    {
        SetEnemyHealthInfo(obj.GetComponent<EnemyHealthController>());
    }

    /// <summary>
    /// playerû�м�⵽����
    /// </summary>
    /// <param name="obj"></param>
    private void OnNotDetectEnemyHandler()
    {
        SetEnemyHealthInfo(null);
    }


    /// <summary>
    /// ����ai����������
    /// </summary>
    /// <param name="obj"></param>
    private void EnemyDeathHandler(Transform obj)
    {
        SetEnemyHealthInfo(null);
    }

    /// <summary>
    /// ����ui������˵�info
    /// </summary>
    /// <param name="enemyHealthController"></param>
    public void SetEnemyHealthInfo(EnemyHealthController enemyHealthController)
    {
        if(enemyHealthController == null)
        {
            CurrentEnemyHealthInfoBox.gameObject.SetActive(false);
            return;
        }
        CurrentEnemyHealthInfoBox.gameObject.SetActive(true);
        CurrentEnemyHealthInfoBox.SetHealthInfo(enemyHealthController);
    }

}
