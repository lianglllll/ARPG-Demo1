using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��ʱ��״̬
/// </summary>
public enum TimerState
{
    NOTWORKERE,//û�й���
    WORKERING,//������
    DONE,     //�������
}


public class GameTimer
{
    //1.��ʱʱ��
    //2.��ʱ������ִ�е�����
    //3.��ǰ��ʱ����״̬
    //4.�Ƿ�ֹͣ��ǰ��ʱ��

    private float _startTime;
    private Action _task;
    private bool _isStopTimer;
    private TimerState _timerState;

    public GameTimer()
    {
        ResetTimer();
    }

    //1.��ʼ��ʱ
    public void StartTimer(float time,Action task)
    {
        _startTime = time;
        _task = task;
        _isStopTimer = false;
        _timerState = TimerState.WORKERING;
    }

    //2.���¼�ʱ��
    public void UpdateTimer()
    {
        if (_isStopTimer) return;
        _startTime -= Time.deltaTime;
        if(_startTime < 0f)
        {
            _task?.Invoke();
            _timerState = TimerState.DONE;
            _isStopTimer = true;
        }
    }

    //3.ȷ����ʱ��״̬
    public TimerState GetTimerState() => _timerState;


    //4.���ü�ʱ��
    public void ResetTimer()
    {
        _startTime = 0f;
        _task = null;
        _isStopTimer = true;
        _timerState = TimerState.NOTWORKERE;
    }


}
