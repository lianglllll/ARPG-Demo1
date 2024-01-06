using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMatchSMB : StateMachineBehaviour
{

    [Header("匹配信息")]
    [SerializeField] private float _startTime;
    [SerializeField] private float _endTime;
    [SerializeField] private AvatarTarget _avatarTarget;
    
    [Header("激活重力")]
    [SerializeField] private bool _isEnableGravity;
    [SerializeField] private float _enableTime;     //动画哪个时间点激活重力

    private Vector3 _matchPosition;
    private Quaternion _matchRotation;

    private void OnEnable()
    {
        GameEventManager.Instance.AddEventListening<Vector3, Quaternion>("SetAnimationMatchInfo", SetAnimationMatchInfo);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveEvent<Vector3, Quaternion>("SetAnimationMatchInfo", SetAnimationMatchInfo);
    }


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.isMatchingTarget)
        {
            animator.MatchTarget(_matchPosition, _matchRotation, _avatarTarget, new MatchTargetWeightMask(Vector3.one, 0f),_startTime,_endTime);
        }
        if (_isEnableGravity)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > _enableTime)
            {
                //激活重力
                GameEventManager.Instance.CallEvent("EnableCharacterGravity", true);
            }
        }
    }


    private void SetAnimationMatchInfo(Vector3 position,Quaternion rotation)
    {
        _matchPosition = position;
        _matchRotation = rotation;
    }


}
