using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{

    private Animator _animator;

    [Header("检测")]
    [SerializeField] private float _detectionDistance;
    [SerializeField] private LayerMask _detectionLayer;

    private RaycastHit _hit;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CharacterClimbInput();
    }

    private bool CanClimb()
    {
        return Physics.Raycast(transform.position + (transform.up * 0.5f), transform.forward, out _hit, _detectionDistance, _detectionLayer, QueryTriggerInteraction.Ignore);
    }

    private void CharacterClimbInput()
    {
        if (!CanClimb()) return;
        if (InputManager.Instance.Climb && !_animator.AnimationAtTag("Climb"))
        {
            //先去获取检测到的墙体信息
            var gapY = _hit.collider.transform.position.y - transform.position.y;      //射线打到目标的点和游戏对象的y差值
            var targetYj = gapY + _hit.collider.bounds.extents.y;                      //y差值+物体的一半===>物体的最高高度
            var position = Vector3.zero;
            var rotation = Quaternion.LookRotation(-_hit.normal);
            position.Set(_hit.point.x, targetYj, _hit.point.z);//获取墙顶点的位置

            //不同的墙
            switch (_hit.collider.tag)
            {
                case "midWall":
                    ToCallEvent(position, rotation);
                    _animator.CrossFade("ClimbMidWall",0f,0,0f);
                    break;
                case "tallWall":
                    ToCallEvent(position, rotation);
                    _animator.CrossFade("ClimbTallWall", 0f, 0, 0f);
                    break;

                     
            }

        }
    }



    private void ToCallEvent(Vector3 position,Quaternion rotation)
    {
        GameEventManager.Instance.CallEvent("SetAnimationMatchInfo",position,rotation);
        GameEventManager.Instance.CallEvent("EnableCharacterGravity", false);
    }
}
  