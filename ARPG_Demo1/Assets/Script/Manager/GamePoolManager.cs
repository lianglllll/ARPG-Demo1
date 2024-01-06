using GGG.Tool;
using GGG.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePoolManager : Singleton<GamePoolManager>
{
    //1.先缓存我们要缓存的对象（我们外面先配置对象）
    //2.缓存
    //3.让外部可以获取对象
    //4.回收对象

    [System.Serializable]
    private class PoolItem
    {
        public string itemName;
        public GameObject item;
        public int initMaxCount;
    }

    [SerializeField]
    private List<PoolItem> _configPoolItem = new List<PoolItem>();
    private Dictionary<string, Queue<GameObject>> _poolCenter = new Dictionary<string, Queue<GameObject>>();
    private GameObject _poolItemParent;


    private void Start()
    {
        _poolItemParent = new GameObject("PoolItemParent");
        _poolItemParent.transform.SetParent(transform);
        InitPool();
    }

    private void InitPool()
    {
        //1.判断外部配置是否为空
        if (_configPoolItem.Count == 0) return;
        for(int i = 0; i < _configPoolItem.Count; i++)
        {
            for(int j = 0; j < _configPoolItem[i].initMaxCount; j++)
            {
                var item = Instantiate(_configPoolItem[i].item);
                item.SetActive(false);                                  //不启用
                item.transform.SetParent(_poolItemParent.transform);          
                //判断池中有没有存在这个对象的key
                if (!_poolCenter.ContainsKey(_configPoolItem[i].itemName))
                {
                    _poolCenter.Add(_configPoolItem[i].itemName, new Queue<GameObject>());
                    _poolCenter[_configPoolItem[i].itemName].Enqueue(item);
                }
                else
                {
                    _poolCenter[_configPoolItem[i].itemName].Enqueue(item);
                }
            }
        }
        //DevelopmentToos.WTF(_poolCenter.Count);
        //DevelopmentToos.WTF(_poolCenter["AttackSound"].Count);
    }

    //申请对象
    public void TryGetPoolItem(string itemName,Vector3 position,Quaternion rotation)
    {
        if (_poolCenter.ContainsKey(itemName))
        {
            var item = _poolCenter[itemName].Dequeue();
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(true);
            _poolCenter[itemName].Enqueue(item);                        //放回尾巴
        }
        else
        {
            DevelopmentToos.WTF("当前申请的对象池不存在：" + itemName);
        }
    }
    public GameObject TryGetPoolItem(string itemName)
    {
        if (_poolCenter.ContainsKey(itemName))
        {
            var item = _poolCenter[name].Dequeue();
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);                        //放回尾巴
            return item;
        }

        DevelopmentToos.WTF("当前申请的对象池不存在：" + itemName);
        return null;
    }

    //回收对象 //todo
}
