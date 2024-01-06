using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPoolItem
{
    void Spawn();
    void Recycle();
}

public abstract class PoolItemBase : MonoBehaviour, IPoolItem
{

    private void OnEnable()
    {
        Spawn();
    }

    private void OnDisable()
    {
        Recycle();
    }


    public virtual void Recycle()
    {

    }

    public virtual void Spawn()
    {

    }
}
