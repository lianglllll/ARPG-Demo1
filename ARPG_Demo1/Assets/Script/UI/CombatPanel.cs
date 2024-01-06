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
        //设置显示自己生命信息的ui
        MyHealthInfoBox.SetHealthInfo(_mainPlayer.GetComponent<PlayerHealthController>());
        SetEnemyHealthInfo(null);


        //监听敌人死亡事件
        GameEventManager.Instance.AddEventListening<Transform>("EnemyDeath", EnemyDeathHandler);
        //监听player检测到敌人
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
    /// player检测到敌人的回调
    /// </summary>
    /// <param name="obj"></param>
    private void OnDetectEnemyHandler(Transform obj)
    {
        SetEnemyHealthInfo(obj.GetComponent<EnemyHealthController>());
    }

    /// <summary>
    /// player没有检测到敌人
    /// </summary>
    /// <param name="obj"></param>
    private void OnNotDetectEnemyHandler()
    {
        SetEnemyHealthInfo(null);
    }


    /// <summary>
    /// 敌人ai死亡处理函数
    /// </summary>
    /// <param name="obj"></param>
    private void EnemyDeathHandler(Transform obj)
    {
        SetEnemyHealthInfo(null);
    }

    /// <summary>
    /// 设置ui所需敌人的info
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
