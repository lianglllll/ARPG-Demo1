using GGG.Tool;
using GGG.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePoolManager : Singleton<GamePoolManager>
{
    //1.�Ȼ�������Ҫ����Ķ����������������ö���
    //2.����
    //3.���ⲿ���Ի�ȡ����
    //4.���ն���

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
        //1.�ж��ⲿ�����Ƿ�Ϊ��
        if (_configPoolItem.Count == 0) return;
        for(int i = 0; i < _configPoolItem.Count; i++)
        {
            for(int j = 0; j < _configPoolItem[i].initMaxCount; j++)
            {
                var item = Instantiate(_configPoolItem[i].item);
                item.SetActive(false);                                  //������
                item.transform.SetParent(_poolItemParent.transform);          
                //�жϳ�����û�д�����������key
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

    //�������
    public void TryGetPoolItem(string itemName,Vector3 position,Quaternion rotation)
    {
        if (_poolCenter.ContainsKey(itemName))
        {
            var item = _poolCenter[itemName].Dequeue();
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(true);
            _poolCenter[itemName].Enqueue(item);                        //�Ż�β��
        }
        else
        {
            DevelopmentToos.WTF("��ǰ����Ķ���ز����ڣ�" + itemName);
        }
    }
    public GameObject TryGetPoolItem(string itemName)
    {
        if (_poolCenter.ContainsKey(itemName))
        {
            var item = _poolCenter[name].Dequeue();
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);                        //�Ż�β��
            return item;
        }

        DevelopmentToos.WTF("��ǰ����Ķ���ز����ڣ�" + itemName);
        return null;
    }

    //���ն��� //todo
}
