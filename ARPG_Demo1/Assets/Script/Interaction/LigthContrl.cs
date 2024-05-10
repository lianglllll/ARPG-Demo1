using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigthContrl : MonoBehaviour
{

    public InteractionBehaviour _light;     //�������д���б�һ�����������ƶ��������Ʒ        
    private bool canCantrol;                //�Ƿ���Խ���

    private void Start()
    {
        canCantrol = false;
    }

    private void Update()
    {
        Control();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canCantrol = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canCantrol = false;
        }
    }

    private void Control()
    {
        if (!canCantrol) return;
        if (_light == null) return;
        if (InputManager.Instance.TakeOut)//E
        {
            _light.InteractionAction();
        }
    }



}
