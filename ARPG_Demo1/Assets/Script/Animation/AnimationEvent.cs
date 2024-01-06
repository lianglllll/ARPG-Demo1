using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Event
{
    public class AnimationEvent : MonoBehaviour
    {
        //这个函数被动画的事件调用，用于用对象池获取声音对象
        private void PlaySound(string soundName)
        {
            GamePoolManager.Instance.TryGetPoolItem(soundName, transform.position, Quaternion.identity);
        }
    }
}
