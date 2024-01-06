using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : InteractionBehaviour
{
    //玩家的交互逻辑
    //比如说开门，只需要八开门的对应逻辑写到一个函数中，然后让其注册到一个事件上，在交互的时候，去呼叫事件即可

    private void OnEnable()
    {
        //订阅事件
    }

    private void OnDisable()
    {
        //取消订阅事件
    }

    //事件触发的时候，要干的事情
    protected override void Interaction()
    {
        throw new System.NotImplementedException();
    }
}
