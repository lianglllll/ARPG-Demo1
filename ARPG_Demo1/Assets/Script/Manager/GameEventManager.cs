using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GGG.Tool;
using GGG.Tool.Singleton;

public class GameEventManager : SingletonNonMono<GameEventManager>
{

    //���ڹ���������
    private interface IEventHelp
    {
    }
   
    /// <summary>
    /// �޲���ί��
    /// </summary>
    private class EventHelp : IEventHelp
    {
        private event Action _action;
        public EventHelp(Action action)
        {
            _action = action;
        }

        //����¼�
        public void AddCall(Action action)
        {
            _action += action;
        }

        //�Ƴ��¼�
        public void RemoveCall(Action action)
        {
            _action -= action;
        }

        //�����¼�
        public void Call()
        {
            _action?.Invoke();
        }

    }
    /// <summary>
    /// ����ί��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class EventHelp<T> : IEventHelp
    {
        private event Action<T> _action;
        public EventHelp(Action<T> action)
        {
            _action = action;
        }

        //����¼�
        public void AddCall(Action<T> action)
        {
            _action += action;
        }

        //�Ƴ��¼�
        public void RemoveCall(Action<T> action)
        {
            _action -= action;
        }

        //�����¼�
        public void Call(T value)
        {
            _action?.Invoke(value);
        }

    }
    /// <summary>
    /// ˫��ί��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class EventHelp<T1,T2> : IEventHelp
    {
        private event Action<T1, T2> _action;
        public EventHelp(Action<T1, T2> action)
        {
            _action = action;
        }

        //����¼�
        public void AddCall(Action<T1, T2> action)
        {
            _action += action;
        }

        //�Ƴ��¼�
        public void RemoveCall(Action<T1, T2> action)
        {
            _action -= action;
        }

        //�����¼�
        public void Call(T1 value1,T2 value2)
        {
            _action?.Invoke(value1,value2);
        }

    }
    /// <summary>
    /// ����
    /// </summary>
    private class EventHelp<T1, T2, T3> : IEventHelp
    {
        private event Action<T1, T2, T3> _action;
        public EventHelp(Action<T1, T2, T3> action)
        {
            _action = action;
        }

        //����¼�
        public void AddCall(Action<T1, T2, T3> action)
        {
            _action += action;
        }

        //�Ƴ��¼�
        public void RemoveCall(Action<T1, T2, T3> action)
        {
            _action -= action;
        }

        //�����¼�
        public void Call(T1 value1, T2 value2, T3 value3)
        {
            _action?.Invoke(value1, value2, value3);
        }

    }
    /// <summary>
    /// ���
    /// </summary>
    private class EventHelp<T1, T2, T3, T4, T5> : IEventHelp
    {
        private event Action<T1, T2, T3, T4, T5> _action;
        public EventHelp(Action<T1, T2, T3, T4, T5> action)
        {
            _action = action;
        }

        //����¼�
        public void AddCall(Action<T1, T2, T3, T4, T5> action)
        {
            _action += action;
        }

        //�Ƴ��¼�
        public void RemoveCall(Action<T1, T2, T3, T4, T5> action)
        {
            _action -= action;
        }

        //�����¼�
        public void Call(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            _action?.Invoke(value1, value2,value3,value4,value5);
        }

    }
    /// <summary>
    /// ����
    /// </summary>
    private class EventHelp<T1, T2, T3, T4, T5,T6> : IEventHelp
    {
        private event Action<T1, T2, T3, T4, T5, T6> _action;
        public EventHelp(Action<T1, T2, T3, T4, T5, T6> action)
        {
            _action = action;
        }

        //����¼�
        public void AddCall(Action<T1, T2, T3, T4, T5, T6> action)
        {
            _action += action;
        }

        //�Ƴ��¼�
        public void RemoveCall(Action<T1, T2, T3, T4, T5, T6> action)
        {
            _action -= action;
        }

        //�����¼�
        public void Call(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5,T6 value6)
        {
            _action?.Invoke(value1, value2, value3, value4, value5, value6);
        }

    }


    private Dictionary<string, IEventHelp> _eventCenter = new Dictionary<string, IEventHelp>();

    //����¼�����:�޲�
    public void AddEventListening(string eventName,Action action)
    {
        //���ж��¼�������û��eventName����¼�
        if(_eventCenter.TryGetValue(eventName,out var e))
        {
            (e as EventHelp)?.AddCall(action);
        }
        else
        {
            //���һ���µ��¼�
            _eventCenter.Add(eventName, new EventHelp(action));
        }
    }
    //����¼�����������
    public void AddEventListening<T>(string eventName, Action<T> action)
    {
        //���ж��¼�������û��eventName����¼�
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T>)?.AddCall(action);
        }
        else
        {
            //���һ���µ��¼�
            _eventCenter.Add(eventName, new EventHelp<T>(action));
        }
    }
    //����¼�������˫��
    public void AddEventListening<T1,T2>(string eventName, Action<T1, T2> action)
    {
        //���ж��¼�������û��eventName����¼�
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2>)?.AddCall(action);
        }
        else
        {
            //���һ���µ��¼�
            _eventCenter.Add(eventName, new EventHelp<T1, T2>(action));
        }
    }
    //����¼�����������
    public void AddEventListening<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
    {
        //���ж��¼�������û��eventName����¼�
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3>)?.AddCall(action);
        }
        else
        {
            //���һ���µ��¼�
            _eventCenter.Add(eventName, new EventHelp<T1, T2, T3>(action));
        }
    }
    //����¼����������
    public void AddEventListening<T1, T2,T3,T4,T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
    {
        //���ж��¼�������û��eventName����¼�
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5>)?.AddCall(action);
        }
        else
        {
            //���һ���µ��¼�
            _eventCenter.Add(eventName, new EventHelp<T1, T2, T3, T4, T5>(action));
        }
    }
    //����¼�����������
    public void AddEventListening<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5,T6> action)
    {
        //���ж��¼�������û��eventName����¼�
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5, T6>)?.AddCall(action);
        }
        else
        {
            //���һ���µ��¼�
            _eventCenter.Add(eventName, new EventHelp<T1, T2, T3, T4, T5, T6>(action));
        }
    }


    //�����¼�
    public void CallEvent(string eventName)
    {
        if(_eventCenter.TryGetValue(eventName,out var e))
        {
            (e as EventHelp)?.Call();
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�");
        }
    }
    //�����¼�������
    public void CallEvent<T>(string eventName,T value)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T>)?.Call(value);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�");
        }
    }
    //�����¼�:˫��
    public void CallEvent<T1,T2>(string eventName,T1 value1,T2 value2)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1,T2>)?.Call(value1,value2);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼����޷�����");
        }
    }
    //�����¼�:����
    public void CallEvent<T1, T2, T3>(string eventName, T1 value1, T2 value2, T3 value3)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3>)?.Call(value1, value2, value3);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼����޷�����");
        }
    }
    //�����¼�:���
    public void CallEvent<T1, T2, T3, T4, T5>(string eventName, T1 value1, T2 value2,T3 value3 ,T4 value4,T5 value5)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5>)?.Call(value1, value2,value3,value4,value5);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼����޷�����");
        }
    }
    //�����¼�:����
    public void CallEvent<T1, T2, T3, T4, T5,T6>(string eventName, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5,T6 value6)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5, T6>)?.Call(value1, value2, value3, value4, value5, value6);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼����޷�����");
        }
    }


    //�Ƴ��¼�
    public void RemoveEvent(string eventName,Action action)
    {
        if(_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp)?.RemoveCall(action);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�,�޷��Ƴ�");
        }
    }

    //�Ƴ��¼�:����
    public void RemoveEvent<T>(string eventName, Action<T> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T>)?.RemoveCall(action);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�,�޷��Ƴ�");
        }
    }

    //�Ƴ��¼�:˫��
    public void RemoveEvent<T1,T2>(string eventName, Action<T1, T2> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2>)?.RemoveCall(action);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�,�޷��Ƴ�");
        }
    }
    //�Ƴ��¼�:����
    public void RemoveEvent<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3>)?.RemoveCall(action);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�,�޷��Ƴ�");
        }
    }
    //�Ƴ��¼�:���
    public void RemoveEvent<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5>)?.RemoveCall(action);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�,�޷��Ƴ�");
        }
    }
    //�Ƴ��¼�:����
    public void RemoveEvent<T1, T2, T3, T4, T5,T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5, T6>)?.RemoveCall(action);
        }
        else
        {
            DevelopmentToos.WTF($"��ǰû���ҵ�{eventName}�¼�,�޷��Ƴ�");
        }
    }

}
