using GGG.Tool;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NCharacterCombatBase : MonoBehaviour
{
    [Header("��������")]
    protected Animator _animator;
    [SerializeField] protected float _animationCrossNormalTime;                     //����ת���Ļ��ʱ��

    [SerializeField]
    protected CharacterComboData _currentComboData;
    [SerializeField]
    protected CharacterComboData DefaultComboData;                                  //Ĭ����ʽ����ʽ��һ������Ϊ��
    private float _attackColdTime;
    protected bool _applyAttackInput;

    [Header("���˷�Χ���")]
    [SerializeField] protected float _detectionRange;
    [SerializeField] private LayerMask _enemyLayer;

    [SerializeField,Header("������Ϣ")]
    protected Transform _currentEnemy;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        _attackColdTime = 0f;
        _applyAttackInput = true;
        _currentComboData = DefaultComboData;
    }

    protected virtual void Update()
    {
        MatchPosition();
        LookTargetOnAttack();
        OnEndAttack();
    }

    #region λ��ͬ��,�ҵ���Ŀ���ʺϲ��Ŷ�����λ��

    protected virtual void MatchPosition()
    {
        if (_currentEnemy == null) return;
        if (_animator == null) return;
        if (_animator.AnimationAtTag("Dead")) return;

        if (_animator.AnimationAtTag("Attack"))
        {
            var timer = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (timer > 0.35f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return;
            RunningMatch(_currentComboData, 0f, 0.45f);
        }
    }

    /// <summary>
    /// λ��ƥ��
    /// </summary>
    /// <param name="combo"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    protected void RunningMatch(CharacterComboData combo, float startTime = 0f, float endTime = 0.1f)
    {
        if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))//��ǰ����ƥ��ͬʱ���ڹ���״̬
        {
            _animator.MatchTarget(_currentEnemy.position + (-transform.forward * combo.GetPositionOffset), Quaternion.identity,
                AvatarTarget.Body, new MatchTargetWeightMask(Vector3.one, 0f), startTime, endTime);
        }

    }

    #endregion

    /// <summary>
    /// ����ʱ����Ŀ��
    protected void LookTargetOnAttack()
    {
        if (_currentEnemy == null) return;
        if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 5f) return;
        //������ʱ�򣬿������
        if (_animator.AnimationAtTag("Attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            transform.Look(_currentEnemy.position, 1000f);
        }
    }

    /// <summary>
    /// ���õ�ǰ����ʽ
    /// </summary>
    /// <param name="characterComboData"></param>
    protected void SetComboData(CharacterComboData characterComboData)
    {
        if (characterComboData == null) return;
        _currentComboData = characterComboData;
    }

    /// <summary>
    /// ��������ִ��,���Ŷ���
    /// </summary>
    protected void ComboActionExecute()
    {
        _attackColdTime = _currentComboData.ActionColdTime;
        _animator.CrossFade(_currentComboData.ActionName, 0.025f, 0, 0f);
        _applyAttackInput = false;
        GameTimerManager.Instance.TryUseOneTimer(_attackColdTime, ResetAttackInput);
    }

    /// <summary>
    /// ���ù�������
    /// </summary>
    protected void ResetAttackInput()
    {
        _applyAttackInput = true;
    }

    /// <summary>
    /// ���������¼������ص�
    /// </summary>
    /// <param name="index">��������index</param>
    private void ATK(int index)
    {
        TrggerDamaged(index);
        PlayATKSound(index);
    }

    /// <summary>
    /// ��ȡ��Χ�ĵ���
    /// </summary>
    /// <returns></returns>
    protected Collider[] GetUnits()
    {
        return Physics.OverlapSphere(transform.position + (transform.up) * 0.7f, _detectionRange, _enemyLayer, QueryTriggerInteraction.Ignore);//û��collider�ļ�ⲻ��
    }

    /// <summary>
    /// ���������Ŀ��
    /// </summary>
    /// <returns></returns>
    protected int LockInRecentTargets(Collider[] enemys)
    {
        if (enemys == null) return -1;
        if (enemys.Length == 0) return -1;
        if (_currentEnemy == null)
        {
            _currentEnemy = enemys[0].transform;
        }else if (_currentEnemy != enemys[0].transform)
        {
            _currentEnemy = enemys[0].transform;
        }
        else
        {
            _currentEnemy = enemys[0].transform;
        }
        GameEventManager.Instance.CallEvent<Transform>("OnDetectEnemy", _currentEnemy);         //������⵽���˵��¼�
        return 0;
    }

    /// <summary>
    /// �����˺������ߵ�����������
    /// 1.�ж��Ƿ񸽼��е���,GetUnits()��ȡ���ӵĵ���
    /// 2.�жϵ��˵ĽǶ��Ƿ���ϴ����˺�
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void TrggerDamaged(int index)
    {
        Collider[] enemys = GetUnits();
        if(LockInRecentTargets(enemys) < 0)
        {
            return;
        }
        //��ֹԽ��
        index = Mathf.Min(index, _currentComboData.DamagedInfos.Count-1);
        //�жϵ��˵ĽǶ�
        foreach(var e in enemys)
        {
            //�������0.85f˵�����˴�����ҵ�ǰ�����������������˺��ĽǶ���
            if (Vector3.Dot(DevelopmentToos.DirectionForTarget(e.transform, transform), transform.forward) > 0.85f) continue;
            if (DevelopmentToos.DistanceForTarget(e.transform, transform) > 2f) continue;
            if(e.transform.TryGetComponent(out IDamage damage))
            {
                damage.CharacterNormalDamage(_currentComboData.DamagedInfos[index].HitName, _currentComboData.DamagedInfos[index].ParryName,
                    _currentComboData.DamagedInfos[index].Damage, transform, _currentComboData.DamagedInfos[index].damagedType);
            }

        }

    }

    /// <summary>
    /// ���ݹ������Ͳ��Ź�����Ч
    /// </summary>
    /// <param name="type"></param>
    public void PlayATKSound(int index)
    {
        DamagedType type = _currentComboData.DamagedInfos[index].damagedType;
        switch (type)
        {
            case DamagedType.PUNCH:
                GamePoolManager.Instance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
                break;
            case DamagedType.WEAPON:
                GamePoolManager.Instance.TryGetPoolItem("SwordSound", transform.position, Quaternion.identity);
                break;
        }
    }
    //������
    public void PlaySound(string str)
    {
        GamePoolManager.Instance.TryGetPoolItem(str, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// ����������������ʽ����Ϊ��һ��
    /// </summary>
    protected void OnEndAttack()
    {
        //�������ƶ�&&���Խ�������Ļ�
        //˵��������һ�ι�������֮�����Ѿ������������ˣ���Ҫ��ʼ����
        if (_animator.AnimationAtTag("Motion") && _applyAttackInput)
        {
            ResetComboData();
        }

    }

    /// <summary>
    /// ������ʽ
    /// </summary>
    protected virtual void ResetComboData()
    {
        //�����Ƿ��������������ʹ���ĸ���ʽ
        _currentComboData = DefaultComboData;
    }

}
