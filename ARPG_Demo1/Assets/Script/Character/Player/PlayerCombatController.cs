using GGG.Tool;
using MyARPG.ComboData;
using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.Combat
{
    public class PlayerCombatController : CharacterCombatBase
    {

        [Header("Player���е���ʽ��")]
        [SerializeField] private CharacterComboSO _changeHeavyCombo;        //�ṥ�����б��ع�����
        [SerializeField] private CharacterComboSO _assassinateCombo;        //��ɱ��
        [SerializeField] private CharacterComboSO _swordCombo;              //������
        private PlayerWpeaons _playerWpeaon;

        [Header("�������")]
        [SerializeField] private float _detectionRange;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Collider[] _units;
        private bool _activeEnemyDetection;                                 //�Ƿ������


        protected override void Awake()
        {
            base.Awake();
            _playerWpeaon = GetComponent<PlayerWpeaons>();
        }

        protected override void Start()
        {
            base.Start();
            _currentCombo = _baseCombo;
            _activeEnemyDetection = true;
        }

        private void OnEnable()
        {
            GameEventManager.Instance.AddEventListening<bool>("EnableFinishEvent", EnableFinishEventHandler);
            GameEventManager.Instance.AddEventListening<Transform>("EnemyDeath", EnemyDeathHandler);
        }

        private void OnDisable()
        {
            GameEventManager.Instance.RemoveEvent<bool>("EnableFinishEvent", EnableFinishEventHandler);
            GameEventManager.Instance.RemoveEvent<Transform>("EnemyDeath", EnemyDeathHandler);

        }

        protected override void Update()
        {
            base.Update();

            ClearEnemy();

            CharacterBaseAtackInput();
            CharacterFinishAttackInput();
            CharacterAssassinationInput();

            //CheackEnemyIsDie();
        }

        private void FixedUpdate()
        {
            GetNearUnitAndSelectEnemyUnit();
        }

        #region Ŀ������ֵ����


        /// <summary>
        /// ���������¼������Ļص�
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyDeathHandler(Transform enemy)
        {
            if(_currentEnemy == enemy)
            {
                _activeEnemyDetection = true;
                _currentEnemy = null;
                _canFinish = false;
            }
        }

        #endregion

        #region λ��ͬ��,�ҵ���Ŀ���ʺϲ��Ŷ�����λ��
        protected override void MatchPosition()
        {
            base.MatchPosition();
            if (_currentEnemy == null) return;
            if (_animator == null) return;
            if (_animator.AnimationAtTag("Dead")) return;

            if (_animator.AnimationAtTag("Finish"))
            {
                transform.rotation = Quaternion.LookRotation(-_currentEnemy.forward);//����Ŀ�꣬dir����enemy������
                RunningMatch(_finishCombo, _finishComboIndex);
            }
            else if (_animator.AnimationAtTag("Assassination"))
            {
                transform.rotation = Quaternion.LookRotation(_currentEnemy.forward);//����Ŀ��ı��棬dir����enemy����ǰ��
                RunningMatch(_assassinateCombo, _finishComboIndex);
            }
        }
        #endregion

        #region ��Χ�Ĺ������

        //1.�����Ϊ���ģ�ȡ�Զ����һ���뾶Բ�ķ�Χ�ڻ�ȡ���еĵ���
        //        ��ǰ��Ŀ�꣬���ٸ���Ŀ�ֱ꣬����ǰĿ����ʧ���˶�ʱ������»�ȡ
        //2.�ڵ�ǰ���û��Ŀ���ʱ��ȡ�������������һ��������ΪĿ�ꡣ
        //        ��ǰĿ��ֻҪ����һ������͸���Ŀ�꣬���Բ���Ҫ�˶�ʱ���
        private void GetNearUnitAndSelectEnemyUnit()
        {
            if (!_activeEnemyDetection) return;
            if (_currentEnemy != null) return;
            _units = Physics.OverlapSphere(transform.position + (transform.up) * 0.7f, _detectionRange, _enemyLayer, QueryTriggerInteraction.Ignore);//û��collider�ļ�ⲻ��
            if (_units.Length == 0)  return;
            if (_animator.AnimationAtTag("Finish")) return;

            Transform tmp_Enemy = null;
            var distance = Mathf.Infinity;
            foreach(var e in _units)
            {
                var dis = DevelopmentToos.DistanceForTarget(e.transform, transform);
                if(dis < distance)
                {
                    tmp_Enemy = e.transform;
                    distance = dis;
                }
            }
            //_currentEnemy = tmp_Enemy != null ? tmp_Enemy : _currentEnemy;
            _currentEnemy = tmp_Enemy;
            _canFinish = false;
            if(_currentEnemy != null)
            {
                GameEventManager.Instance.CallEvent<Transform>("OnDetectEnemy", _currentEnemy);
            }
        }

        /// <summary>
        /// ���Ŀ��,���Χ���,��һ���õ�
        /// </summary>
        private void ClearEnemy()
        {
            if (_currentEnemy == null) return;
            //��ɫ�ƶ���ʱ��
            if (_animator.GetFloat(AnimationID.MovementID) > 0.7f && DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 3f) 
            {
                _currentEnemy = null;
                _canFinish = false;
                GameEventManager.Instance.CallEvent("OnNotDetectEnemy");
            }
        }

        /// <summary>
        /// test
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + (transform.up * 0.7f)  , _detectionRange);
        }

        #endregion

        #region ��ɫ������������

        private bool CanBaseAttackInput()
        {
            //1._canAttackInput == false ��������
            //2. �����ɫ���ڰ��ᣬ��������
            //3. ��ɫ���ڸ񵲣���������
            //4.��ɫ���ڴ�������������
            if (_canAttackInput == false) return false;
            if (_animator.AnimationAtTag("Hit")) return false;
            if (_animator.AnimationAtTag("Parry")) return false;
            if (_animator.AnimationAtTag("Execute")) return false;
            if (_animator.AnimationAtTag("Finish")) return false;
            if (_animator.AnimationAtTag("Assassination")) return false;
            return true;
        }

        protected override void CharacterBaseAtackInput()
        {
            if (!CanBaseAttackInput()) return;

            //�������������߼�
            if (InputManager.Instance.LAttack) 
            {
                if (_playerWpeaon.WeaponIsShow() && _currentCombo != _swordCombo)
                {
                    ChangeCombo(_swordCombo);
                }
                //1.�жϵ�ǰ����ϼ��Ƿ�Ϊ�ջ��߲�λ������ϼ�
                else if (!_playerWpeaon.WeaponIsShow() && _currentCombo != _baseCombo)
                {
                    ChangeCombo(_baseCombo);
                }
                //ִ��
                ExecuteComboAction();
            }
            //���У���������
            else if (InputManager.Instance.RAttack)
            {
                if(_currentComboAttackCount > 2)
                {
                    ChangeCombo(_changeHeavyCombo);
                    switch (_currentComboAttackCount)
                    {

                        case 3:
                            _currentComboIndex = 1;
                            break;
                        default:
                            _currentComboIndex = 0;
                            break;
                    }
                    //ִ��
                    ExecuteComboAction();
                    _currentComboAttackCount = 0;
                }

            }


        }



        #endregion

        #region ����


        /// <summary>
        /// �Ƿ���Դ���,�Ʒ���ʱ����Դ���������ֵ����ĳ��ֵҲ���Դ���
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
            if (!_canFinish) return false;
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
                //1.ȥ���Ŷ�Ӧ�Ĵ����������������ȡһ������������
                _finishComboIndex = Random.Range(0, _finishCombo.TryGetComboMaxCount());
                _animator.Play(_finishCombo.TryGetOneComboAction(_finishComboIndex));
                //2.�����¼����ģ����õ���ע��Ĵ����¼�,���䲥�ű���������
                GameEventManager.Instance.CallEvent("OnFinishAttack", _finishCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                //�����¼�����,�����ע�ӵ���
                GameEventManager.Instance.CallEvent("SetMainCameraTarget", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);
                ResetComboInfo();
                _currentComboAttackCount = 0;
                EnemyManager.Instance.StopAllActioveUnit();
                //������ɺ�
                _canFinish = false;
                //����ʱ�رյ��˼��
                _activeEnemyDetection = false ;

            }
        }

        /// <summary>
        /// �¼������ĵз� ���Ա��������¼�
        /// </summary>
        private void EnableFinishEventHandler(bool apply)
        {
            if (_canFinish) return;
            _canFinish = apply;
        }




        #endregion

        #region ��ɱ

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


        private void CharacterAssassinationInput()
        {
            if (!CanAssassination()) return;
            if (InputManager.Instance.TakeOut)
            {
                //1.ִ�а�ɱ����
                _finishComboIndex = Random.Range(0, _assassinateCombo.TryGetComboMaxCount());
                _animator.Play(_assassinateCombo.TryGetOneComboAction(_finishComboIndex),0,0f);
                //2.�����¼����ģ�֪ͨ���˱���ɱ
                GameEventManager.Instance.CallEvent("OnFinishAttack", _assassinateCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                ResetComboInfo();
            }
        }


        #endregion

        #region ������


        /// <summary>
        /// �����������������ǵ���ײ��ʱ���͸���һ����ǰ����
        /// </summary>
        /// <param name="hit"></param>
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody rigidbody;
            if(hit.transform.TryGetComponent(out rigidbody))
            {
                rigidbody.AddForce(transform.forward * 20f, ForceMode.Force);
            }

        }
        #endregion

       
    }

}
