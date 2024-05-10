using GGG.Tool;
using MyARPG.ComboData;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Combat
{
    public class CharacterCombatBase : MonoBehaviour
    {
        //��Һ�ai���ǵĹ�����������ͬ��
        //�˺�����Ҳ����ͬ��
        //���ǵĻ�����ϼ�Ҳ�ǲ���
        //��ϼ�����Ϣ����

        [Header("��������")]
        protected Animator _animator;
        [SerializeField] protected float _animationCrossNormalTime;                     //����ת���Ļ��ʱ��

        [Header("��������ʽ��")]
        [SerializeField] protected CharacterComboSO _baseCombo;                         //����������ʽ��
        [SerializeField] protected CharacterComboSO _finishCombo;                       //������
        protected CharacterComboSO _currentCombo;                                       //��ǰ����ʽ��
        protected int _currentComboIndex;                                               //��ǰ��ʽ���е���ʽ����
        protected float _maxColdTime;                                                   //�����������ʱ��
        protected bool _canAttackInput;                                                 //�Ƿ��������빥���ź�
        protected int _hitIndex;                                                        //��ǰ��ʽ��Ķ���������������hit��parry����
        protected int _currentComboAttackCount;                                         //��ǰ��������,���ڱ����ã������ء������ᡢ����������
        protected ComboDataType _currentComboDataType;


        //����
        protected int _finishComboIndex;
        protected bool _canFinish;


        [Header("������Ϣ")]
        [SerializeField] protected Transform _currentEnemy;


        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            _canAttackInput = true;
            _currentCombo = _baseCombo;
            _canFinish = false;
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
                RunningMatch(_currentCombo, _currentComboIndex, 0f, 0.45f);
            }
        }

        /// <summary>
        /// λ��ƥ��
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        protected void RunningMatch(CharacterComboSO combo, int index, float startTime = 0f, float endTime = 0.1f)
        {
            if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))//��ǰ����ƥ��ͬʱ���ڹ���״̬
            {
                _animator.MatchTarget(_currentEnemy.position + (-transform.forward * combo.TryGetComboPositionOffset(index)), Quaternion.identity,
                    AvatarTarget.Body, new MatchTargetWeightMask(Vector3.one, 0f), startTime, endTime);
            }

        }

        #endregion

        #region ������������
        protected virtual void CharacterBaseAtackInput()
        {

        }
        /// <summary>
        /// �ı����б�
        /// </summary>
        /// <param name="combo"></param>
        protected void ChangeCombo(CharacterComboSO combo)
        {
            if (_currentCombo != combo)
            {
                _currentCombo = combo;
                ResetComboInfo();
            }
        }
        #endregion

        #region ��ʽִ�У���������
        protected virtual void ExecuteComboAction()
        {
            //���µ�ǰ��ʽ�Ķ���HitIndex����ֵ
            _hitIndex = 0;
            _currentComboAttackCount += (_currentCombo == _baseCombo) ? 1 : 0;
            _currentComboDataType = _currentCombo.GetComboDataType(_currentComboIndex);

            _maxColdTime = _currentCombo.TryGetComboColdTime(_currentComboIndex);
            _animator.CrossFadeInFixedTime(_currentCombo.TryGetOneComboAction(_currentComboIndex), 0.15555f, 0, 0f);//���Ŷ���
            GameTimerManager.Instance.TryUseOneTimer(_maxColdTime, UpdateComboInfo);        //ע���������UpdateComboInfo�������ȴ�¼����ϣ����ܻᵼ��_currentComboIndex��++��������Խ��
            _canAttackInput = false; //���ù������룬�ȵ������ĸ�����������Ŵ�

        }
        #endregion

        #region ����ʱ����Ŀ��
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

        #endregion

        #region �����¼�������������
        /// <summary>
        /// �����¼������Ĺ����¼�
        /// </summary>
        protected void ATK()
        {
            TriggerDamage();
            UpdateHitIndex();
            PlayATKSound(_currentComboDataType);
        }
        public void PlayATKSound(ComboDataType type)
        {
            switch (type)
            {
                case ComboDataType.PUNCH:
                case ComboDataType.TICK:
                    GamePoolManager.Instance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
                    break;
                case ComboDataType.WEAPON:
                    GamePoolManager.Instance.TryGetPoolItem("SwordSound", transform.position, Quaternion.identity);
                    break;
            }
        }

        protected void LastFinishATK()
        {
            TriggerFinishDamage();
            UpdateHitIndex();
            GamePoolManager.Instance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
        }


        #endregion

        #region �˺�����


        /// <summary>
        /// ÿ����ʽ�еĶ���������һ��
        /// </summary>
        protected void TriggerDamage()
        {

            //1.����Ҫȷ����Ŀ��
            if (_currentEnemy == null) return;
            //2.Ҫȷ�����˴������ǿɴ����˺��ľ���ͽǶ�
            if (Vector3.Dot(transform.forward, DevelopmentToos.DirectionForTarget(_currentEnemy, transform)) > 0.85f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 1.3f) return;

            //3.ȥ�����¼����ģ����ҵ��ã������˺��������
            if (_animator.AnimationAtTag("Attack"))
            {
                GameEventManager.Instance.CallEvent("OnCharacterHitEvent",
                    _currentCombo.TryGetComboDamage(_currentComboIndex),
                    _currentCombo.TryGetOneHitName(_currentComboIndex, _hitIndex),
                    _currentCombo.TryGetOneParryName(_currentComboIndex, _hitIndex),
                    transform, _currentEnemy,_currentCombo.GetComboDataType(_currentComboIndex));
                //���ﴫ�����˶����ǵ�������Ƭ��
            }
            else
            {
                //������������ɱ����
                //������һ�������ı�����������ͬһ�������ڼ䣬�ᴥ������˺�
                GameEventManager.Instance.CallEvent("OnFinishDamage", _currentEnemy);
            }

        }

        protected void TriggerFinishDamage()
        {
            //1.����Ҫȷ����Ŀ��
            if (_currentEnemy == null) return;
            //2.Ҫȷ�����˴������ǿɴ����˺��ľ���ͽǶ�
            if (Vector3.Dot(transform.forward, DevelopmentToos.DirectionForTarget(_currentEnemy, transform)) > 0.85f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 1.3f) return;
            GameEventManager.Instance.CallEvent("OnLastFinishDamage", _finishCombo.TryGetComboDamage(_finishComboIndex), _currentEnemy);
        }

        #endregion

        #region ������ʽ��Ϣ

        //timer������ɵĻص�:�����������ִ��
        protected virtual void UpdateComboInfo()
        {
            _currentComboIndex++;
            if (_currentComboIndex == _currentCombo.TryGetComboMaxCount())
            {
                //�����ǰ�Ĺ��������Ѿ�ִ�е����һ������(��ʽ)
                _currentComboIndex = 0;
            }
            _maxColdTime = 0;
            _canAttackInput = true;
        }

        protected void UpdateHitIndex()
        {
            _hitIndex++;
            if (_hitIndex >= _currentCombo.TryGetHitOrParryMaxCount(_currentComboIndex))
            {
                _hitIndex = 0;
            }

        }
        #endregion

        #region ������ʽ��Ϣ

        protected void ResetComboInfo()
        {
            _currentComboIndex = 0;
            _maxColdTime = 0f;
            _hitIndex = 0;
        }

        /// <summary>
        /// ����������������������һ�¹�������
        /// </summary>
        protected void OnEndAttack()
        {
            //�������ƶ�&&���Խ�������Ļ�
            //˵��������һ�ι�������֮�����Ѿ������������ˣ���Ҫ��ʼ����
            if (_animator.AnimationAtTag("Motion") && _canAttackInput)
            {
                ResetComboInfo();
                _currentComboAttackCount = 0;
            }
        }


        #endregion





    }
}
