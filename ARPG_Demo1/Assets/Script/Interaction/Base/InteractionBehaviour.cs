using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class InteractionBehaviour : MonoBehaviour, IInteraction
{
    protected bool _canInteraction;

    protected virtual void Start()
    {
        _canInteraction = true;
    }

    public bool CanInteraction()
    {
        return _canInteraction;
    }

    /// <summary>
    /// Ä£°å·½·¨
    /// </summary>
    public void InteractionAction()
    {
        Interaction();
    }
    protected abstract void Interaction();

}
