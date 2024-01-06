using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Event
{
    public class AnimationEvent : MonoBehaviour
    {
        //����������������¼����ã������ö���ػ�ȡ��������
        private void PlaySound(string soundName)
        {
            GamePoolManager.Instance.TryGetPoolItem(soundName, transform.position, Quaternion.identity);
        }
    }
}
