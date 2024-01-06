using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{

    [SerializeField, Header("�����Сƫ����")]          
    private Vector2 _maxDistanceOffset;                     //�Ǻ����ĸ�����Ϊ����������ƫ��,��С������1
    [SerializeField, Header("���㼶"), Space(10)]
    private LayerMask _whatIsWall;
    [SerializeField, Header("���߳���"), Space(10)] 
    private float _detectionDistance;
    [SerializeField, Header("��ײ�ƶ�ƽ��ʱ��"), Space(10)]
    private float _colliderSmoothTime;

    //��ʼ��ʱ����Ҫ������ʼ�����ʼ��ƫ��
    private Vector3 _originPosition;                                //������û���(0,0,-1)
    private float _originOffsetDistance;
    private Transform _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        UpadateCollider();
    }

    private void Start()
    {
        _originPosition = transform.localPosition.normalized;//normalized��һ��֮�����(0,0-1),Ҳ��������TP_camera������ĺ�
        _originOffsetDistance = _maxDistanceOffset.y;
    }

    /// <summary>
    /// �����ײ
    /// </summary>
    private void UpadateCollider()
    {
        var detectionDirection = transform.TransformPoint(_originPosition * _detectionDistance);                //����ת���ǽ����Ը�����Ϊ���ĵģ�0��0��-1��ת��Ϊ�������길�������󷽵ķ���
        if(Physics.Linecast(transform.position,detectionDirection,out var hit, _whatIsWall, QueryTriggerInteraction.Ignore))
        {
            //�򵽶�������˵����ײ�����������������ǰ�ƶ�һ�ξ���
            _originOffsetDistance = Mathf.Clamp(hit.distance *0.8f, _maxDistanceOffset.x, _maxDistanceOffset.y);
        }
        else
        {
            _originOffsetDistance = _maxDistanceOffset.y;
        }
        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _originPosition * (_originOffsetDistance - 0.1f), DevelopmentToos.UnTetheredLerp(_colliderSmoothTime));
    }

}


