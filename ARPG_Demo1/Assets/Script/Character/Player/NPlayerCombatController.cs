using GGG.Tool;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPlayerCombatController : NCharacterCombatBase
{
    private PlayerWpeaons _playerWpeaon;

    [SerializeField,Header("��չ��ʽ")]
    protected CharacterComboData SwordComboData;                                    //������ʽ
    [SerializeField]
    protected CharacterComboData FinishComboData;                                   //������ʽ    
    [SerializeField]
    protected CharacterComboData AssassinateComboData;                              //��ɱ��ʽ


    protected bool CanFinish
    {
        get
        {
            if (_currentEnemy == null) return false;
            return _currentEnemy.GetComponent<CharacterHealthBase>().CanFinish();
        }
    }


    protected override void Awake()
    {
        base.Awake();
        _playerWpeaon = GetComponent<PlayerWpeaons>();
    }

    protected override void Update()    
    {
        base.Update();
        ClearEnemy();
        PlayerAttackInput();
        CharacterFinishAttackInput();
        CharacterAssassinationInput();
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddEventListening<Transform>("EnemyDeath", EnemyDeathHandler);
    }
    private void OnDisable()
    {
        GameEventManager.Instance.RemoveEvent<Transform>("EnemyDeath", EnemyDeathHandler);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (transform.up * 0.7f), _detectionRange);
    }

    /// <summary>
    /// ���Ŀ��
    /// </summary>
    private void ClearEnemy()
    {
        if (_currentEnemy == null) return;
        //��ɫ�ƶ���ʱ��
        if (_animator.GetFloat(AnimationID.MovementID) > 0.7f && DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 3f)
        {
            _currentEnemy = null;
            GameEventManager.Instance.CallEvent("OnNotDetectEnemy");                        //�����������ɫ�¼�
        }
    }

    /// <summary>
    /// λ��ͬ��,�ҵ���Ŀ���ʺϲ��Ŷ�����λ��
    /// </summary>
    protected override void MatchPosition()
    {
        base.MatchPosition();
        if (_currentEnemy == null) return;
        if (_animator == null) return;
        if (_animator.AnimationAtTag("Dead")) return;

        if (_animator.AnimationAtTag("Finish"))
        {
            transform.rotation = Quaternion.LookRotation(-_currentEnemy.forward);//����Ŀ�꣬dir����enemy������
            RunningMatch(FinishComboData);
        }
        else if (_animator.AnimationAtTag("Assassination"))
        {
            transform.rotation = Quaternion.LookRotation(_currentEnemy.forward);//����Ŀ��ı��棬dir����enemy����ǰ��
            RunningMatch(AssassinateComboData);
        }
    }


    /// <summary>
    /// �Ƿ������ͨ����
    /// </summary>
    /// <returns></returns>
    private bool CanBaseAttackInput()
    {
        //1._canAttackInput == false ��������
        //2. �����ɫ���ڰ��ᣬ��������
        //3. ��ɫ���ڸ񵲣���������
        //4.��ɫ���ڴ�������������
        if (_applyAttackInput == false) return false;
        if (_animator.AnimationAtTag("Hit")) return false;
        if (_animator.AnimationAtTag("Parry")) return false;
        if (_animator.AnimationAtTag("Execute")) return false;
        if (_animator.AnimationAtTag("Finish")) return false;
        if (_animator.AnimationAtTag("Assassination")) return false;
        return true;
    }

    /// <summary>
    /// ��ҵ���ͨ��������
    /// </summary>
    private void PlayerAttackInput()
    {
        if (!CanBaseAttackInput()) return;
        if (InputManager.Instance.LAttack)              //��������
        {   
            SetComboData(_currentComboData.NextComboData);
            ComboActionExecute();
        }
        else if (InputManager.Instance.RAttack)         //��������
        {
            if (!_currentComboData.HasChildComboData) return;
            SetComboData(_currentComboData.ChildComboData);
            ComboActionExecute();
        }
    }

    /// <summary>
    /// �Ƿ���Դ���,����ֵ����ĳ��ֵҲ���Դ���
    /// </summary>
    /// <returns></returns>
    private bool CanSpecialAttack()
    {
        if (_currentEnemy == null) return false;
        if (_animator.AnimationAtTag("Parry")) return false;
        if (_animator.AnimationAtTag("Dash")) return false;
        if (_animator.AnimationAtTag("Hit")) return false;
        if (_animator.AnimationAtTag("Finish")) return false;            //Finish:����
        if (_animator.AnimationAtTag("Assassination")) return false;
        if (!CanFinish) return false;
        return true;
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void CharacterFinishAttackInput()
    {
        if (!CanSpecialAttack()) return;
        if (InputManager.Instance.Grab)
        {
            //���˹�����ȡ��ȫ������ָ��
            EnemyManager.Instance.StopAllActioveUnit();

            //1.ȥ���Ŷ�Ӧ�Ĵ�������
            _animator.Play(FinishComboData.ActionName);
            //2.���ߵ�����Ҫ�������ˣ��о����Ŷ���
            if (_currentEnemy.transform.TryGetComponent(out IDamage damage))
            {
                damage.BeginFinished(FinishComboData.DamagedInfos[0].HitName, transform);
            }
            //3.�����¼�����,�����ע�ӵ���
            GameEventManager.Instance.CallEvent("SetMainCameraTarget", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);
            //4.������ɺ�
            ResetComboData();
        }
    }

    /// <summary>
    /// �Ƿ���԰�ɱ
    /// </summary>
    /// <returns></returns>
    private bool CanAssassination()
    {
        //1.��ǰû��Ŀ��
        if (_currentEnemy == null) return false;
        //2.����̫Զ
        if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return false;
        //3.�Ƕ�̫��
        if (Vector3.Angle(transform.forward, _currentEnemy.forward) > 30f) return false;
        //4.����������ڰ�ɱ
        if (_animator.AnimationAtTag("Assassination")) return false;
        //5.�����ڴ���
        if (_animator.AnimationAtTag("Finish")) return false;
        return true;
    }

    /// <summary>
    /// ��ɱ����
    /// </summary>
    private void CharacterAssassinationInput()
    {

        if (InputManager.Instance.TakeOut)
        {
            //�Զ�����
            LockInRecentTargets(GetUnits());
            if (!CanAssassination()) return;

            //���˹�����ȡ��ȫ������ָ��
            EnemyManager.Instance.StopAllActioveUnit();

            //1.ȥ���Ŷ�Ӧ�Ĵ�������
            _animator.Play(AssassinateComboData.ActionName);
            //2.���ߵ�����Ҫ�������ˣ��о����Ŷ���
            if (_currentEnemy.transform.TryGetComponent(out IDamage damage))
            {
                damage.BeginFinished(AssassinateComboData.DamagedInfos[0].HitName, transform);
            }
            //3.�����¼�����,�����ע�ӵ���
            GameEventManager.Instance.CallEvent("SetMainCameraTarget", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);
            //4.������ɺ�
            ResetComboData();
        }
    }

    /// <summary>
    /// �������������˺�
    /// </summary>
    /// <param name="index"></param>
    protected void FinishATK(int index)
    {
        //1.����Ҫȷ����Ŀ��
        if (_currentEnemy == null) return;
        _currentEnemy.GetComponent<CharacterHealthBase>().FinishDamage(FinishComboData.DamagedInfos[0].Damage, FinishComboData.DamagedInfos[0].damagedType, index==1);
    }

    /// <summary>
    /// ���������¼������Ļص�
    /// </summary>
    /// <param name="enemy"></param>
    public void EnemyDeathHandler(Transform enemy)
    {
        if (_currentEnemy == enemy)
        {
            _currentEnemy = null;
        }
    }

    /// <summary>
    /// ������
    /// �����������������ǵ���ײ��ʱ���͸���һ����ǰ����
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody;
        if (hit.transform.TryGetComponent(out rigidbody))
        {
            rigidbody.AddForce(transform.forward * 20f, ForceMode.Force);
        }

    }
}
