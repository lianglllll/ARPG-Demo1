using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigthContrl : MonoBehaviour
{

    public InteractionBehaviour _light;     //这个可以写成列表，一个控制器控制多个交互物品        
    private bool canCantrol;                //是否可以交互

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
