using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_CameraController : MonoBehaviour
{
    [Header("�����������")]
    private Transform _lookTarget;
    [SerializeField] private float _positionOffset = 0.1f;                                  //�����_currentLookTarget��ƫ��ֵ
    [SerializeField] private float _controllerSpeed = 0.3f;                                 //������ƶ��ٶ�
    [SerializeField] private float _positionSmoothTime = 10;                                //����ƶ�ƽ��ʱ��
    [SerializeField] private float _rotateSmoothTime = 0.1f;                                //�����תƽ��ʱ��
    [SerializeField] private Vector2 _cameraVerticalMaxAngle = new Vector2(-65,65);   //����������¿������Ƕ�

    private Vector3 _currentRotateVelocity = Vector3.zero;                                  //��ǰ������ƶ��ٶ�,��������Ϊ0
    private Vector2 _input;                                                                 //���ڽ����������
    private Vector3 _cameraRotation;                                                        //���ڱ������������תֵ
    private Transform _currentLookTarget;                                                   //�������ǰע�͵�Ŀ��
    private bool _isFinish;                                                                 //�Ƿ������������ģʽ


    private void Awake()
    {
        _lookTarget = GameObject.FindWithTag("CameraTarget").transform;     //player���Ϲҵ�
        _currentLookTarget = _lookTarget;
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddEventListening<Transform, float>("SetMainCameraTarget", SetFnishTarget);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveEvent<Transform, float>("SetMainCameraTarget", SetFnishTarget);
    }


    private void Start()
    {
        //�������
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _isFinish = false;
    }
    
    private void Update()
    {
        CameraInput();
    }

    /// <summary>
    /// LateUpdate ͨ�����ڴ����������λ�úͷ�����Ϊ�� Update �׶��п��ܻ�������������˶����� LateUpdate ��ȷ�������������֮ǰ������Щ�仯��
    /// </summary>
    private void LateUpdate()
    {
        UpdateCameraRotation();
        CameraPosition();
    }

    /// <summary>
    /// ��ȡ�������
    /// </summary>
    private void CameraInput()
    {
        if (_isFinish) return;
        _input.y += InputManager.Instance.CameraLook.x * _controllerSpeed;              //���ҿ�����ת�����y��
        _input.x -= InputManager.Instance.CameraLook.y * _controllerSpeed;              //���¿�����ת�����x��
        //��������
        _input.x = Mathf.Clamp(_input.x, _cameraVerticalMaxAngle.x, _cameraVerticalMaxAngle.y);
    }

    /// <summary>
    /// �����������ת
    /// </summary>
    private void UpdateCameraRotation()
    {
        _cameraRotation = Vector3.SmoothDamp(_cameraRotation, new Vector3(_input.x, _input.y, 0f),ref _currentRotateVelocity, _rotateSmoothTime);
        transform.eulerAngles = _cameraRotation;
    }

    /// <summary>
    /// ���������λ��
    /// </summary>
    private void CameraPosition()
    {
        //var newPosition = (_currentLookTarget.position + (-_currentLookTarget.transform.forward * _positionOffset));            //��_currentLookTargetλ��Ϊ��׼������ƶ�_positionOffset
        var newPosition = (((_isFinish)? _currentLookTarget.transform.position + _currentLookTarget.up*0.9f : _currentLookTarget.position) + (-_currentLookTarget.transform.forward * _positionOffset));
        transform.position = Vector3.Lerp(transform.position, newPosition, DevelopmentToos.UnTetheredLerp(_positionSmoothTime));
    }

    /// <summary>
    /// player��������ʱ�Ļص�
    /// ��Ҫ�õ�ǰ�������ע�ӵ���
    /// </summary>
    /// <param name="target"></param>
    /// <param name="time"></param>
    private void SetFnishTarget(Transform target,float time)
    {
        _isFinish = true;
        _currentLookTarget = target;
        GameTimerManager.Instance.TryUseOneTimer(time, ResetTarget);
    }

    /// <summary>
    /// ���������ע�ӵ�Ŀ��
    /// </summary>
    private void ResetTarget()
    {
        _isFinish = false;
        _currentLookTarget = _lookTarget;
    }

}
    