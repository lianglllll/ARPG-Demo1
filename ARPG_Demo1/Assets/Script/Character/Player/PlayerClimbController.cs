using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{

    private Animator _animator;

    [Header("���")]
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
            //��ȥ��ȡ��⵽��ǽ����Ϣ
            var gapY = _hit.collider.transform.position.y - transform.position.y;      //���ߴ�Ŀ��ĵ����Ϸ�����y��ֵ
            var targetYj = gapY + _hit.collider.bounds.extents.y;                      //y��ֵ+�����һ��===>�������߸߶�
            var position = Vector3.zero;
            var rotation = Quaternion.LookRotation(-_hit.normal);
            position.Set(_hit.point.x, targetYj, _hit.point.z);//��ȡǽ�����λ��

            //��ͬ��ǽ
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
  