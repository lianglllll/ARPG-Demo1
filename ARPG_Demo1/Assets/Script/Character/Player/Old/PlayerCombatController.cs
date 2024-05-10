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

        [Header("Player特有的招式表")]
        [SerializeField] private CharacterComboSO _changeHeavyCombo;        //轻攻击变招表（重攻击）
        [SerializeField] private CharacterComboSO _assassinateCombo;        //暗杀表
        [SerializeField] private CharacterComboSO _swordCombo;              //武器表
        private PlayerWpeaons _playerWpeaon;

        [Header("攻击检测")]
        [SerializeField] private float _detectionRange;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Collider[] _units;
        private bool _activeEnemyDetection;                                 //是否检测敌人


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

        #region 目标生命值监听


        /// <summary>
        /// 敌人死亡事件触发的回调
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

        #region 位置同步,找到和目标适合播放动画的位置
        protected override void MatchPosition()
        {
            base.MatchPosition();
            if (_currentEnemy == null) return;
            if (_animator == null) return;
            if (_animator.AnimationAtTag("Dead")) return;

            if (_animator.AnimationAtTag("Finish"))
            {
                transform.rotation = Quaternion.LookRotation(-_currentEnemy.forward);//看向目标，dir就是enemy的正后方
                RunningMatch(_finishCombo, _finishComboIndex);
            }
            else if (_animator.AnimationAtTag("Assassination"))
            {
                transform.rotation = Quaternion.LookRotation(_currentEnemy.forward);//看向目标的背面，dir就是enemy的正前方
                RunningMatch(_assassinateCombo, _finishComboIndex);
            }
        }
        #endregion

        #region 范围的攻击检测

        //1.以玩家为中心，取自定义的一个半径圆的范围内获取其中的敌人
        //        当前有目标，不再更新目标，直到当前目标消失或运动时清空重新获取
        //2.在当前玩家没有目标的时候，取距离自身最近的一名敌人作为目标。
        //        当前目标只要大于一定距离就更新目标，所以不需要运动时清空
        private void GetNearUnitAndSelectEnemyUnit()
        {
            if (!_activeEnemyDetection) return;
            if (_currentEnemy != null) return;
            _units = Physics.OverlapSphere(transform.position + (transform.up) * 0.7f, _detectionRange, _enemyLayer, QueryTriggerInteraction.Ignore);//没有collider的检测不到
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
        /// 清除目标,激活范围检测,第一种用的
        /// </summary>
        private void ClearEnemy()
        {
            if (_currentEnemy == null) return;
            //角色移动的时候
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

        #region 角色基础攻击输入

        private bool CanBaseAttackInput()
        {
            //1._canAttackInput == false 不允许攻击
            //2. 如果角色正在挨揍，不允许攻击
            //3. 角色正在格挡，不允许攻击
            //4.角色正在处决，不允许攻击
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

            //正常攻击输入逻辑
            if (InputManager.Instance.LAttack) 
            {
                if (_playerWpeaon.WeaponIsShow() && _currentCombo != _swordCombo)
                {
                    ChangeCombo(_swordCombo);
                }
                //1.判断当前的组合技是否为空或者部位基础组合技
                else if (!_playerWpeaon.WeaponIsShow() && _currentCombo != _baseCombo)
                {
                    ChangeCombo(_baseCombo);
                }
                //执行
                ExecuteComboAction();
            }
            //变招：轻轻轻重
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
                    //执行
                    ExecuteComboAction();
                    _currentComboAttackCount = 0;
                }

            }


        }



        #endregion

        #region 处决


        /// <summary>
        /// 是否可以处决,破防的时候可以处决，生命值低于某个值也可以处决
        /// </summary>
        /// <returns></returns>
        private bool CanSpecialAttack()
        {
            if (_currentEnemy == null) return false;
            if (_animator.AnimationAtTag("Parry")) return false;
            if (_animator.AnimationAtTag("Dash")) return false;
            if (_animator.AnimationAtTag("Hit")) return false;
            if (_animator.AnimationAtTag("Finish")) return false;            //Finish:处决
            if (_animator.AnimationAtTag("Assassination")) return false;
            if (!_canFinish) return false;
            return true;
        }

        /// <summary>
        /// 处决输入
        /// </summary>
        private void CharacterFinishAttackInput()
        {
            if (!CanSpecialAttack()) return;
            if (InputManager.Instance.Grab)
            {
                //1.去播放对应的处决动画（这里随机取一个处决动作）
                _finishComboIndex = Random.Range(0, _finishCombo.TryGetComboMaxCount());
                _animator.Play(_finishCombo.TryGetOneComboAction(_finishComboIndex));
                //2.呼叫事件中心，调用敌人注册的处决事件,让其播放被处决动画
                GameEventManager.Instance.CallEvent("OnFinishAttack", _finishCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                //呼叫事件中心,将相机注视敌人
                GameEventManager.Instance.CallEvent("SetMainCameraTarget", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);
                ResetComboInfo();
                _currentComboAttackCount = 0;
                EnemyManager.Instance.StopAllActioveUnit();
                //处决完成后
                _canFinish = false;
                //处决时关闭敌人检测
                _activeEnemyDetection = false ;

            }
        }

        /// <summary>
        /// 事件，订阅敌方 可以被处决的事件
        /// </summary>
        private void EnableFinishEventHandler(bool apply)
        {
            if (_canFinish) return;
            _canFinish = apply;
        }




        #endregion

        #region 暗杀

        private bool CanAssassination()
        {
            //1.当前没有目标
            if (_currentEnemy == null) return false; 
            //2.距离太远
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return false;
            //3.角度太大
            if (Vector3.Angle(transform.forward, _currentEnemy.forward) > 30f) return false;
            //4.如果现在正在暗杀
            if (_animator.AnimationAtTag("Assassination")) return false;
            //5.现在在处决
            if (_animator.AnimationAtTag("Finish")) return false;
            return true;
        }


        private void CharacterAssassinationInput()
        {
            if (!CanAssassination()) return;
            if (InputManager.Instance.TakeOut)
            {
                //1.执行暗杀动画
                _finishComboIndex = Random.Range(0, _assassinateCombo.TryGetComboMaxCount());
                _animator.Play(_assassinateCombo.TryGetOneComboAction(_finishComboIndex),0,0f);
                //2.呼叫事件中心，通知敌人被暗杀
                GameEventManager.Instance.CallEvent("OnFinishAttack", _assassinateCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                ResetComboInfo();
            }
        }


        #endregion

        #region 物理交互


        /// <summary>
        /// 当其他物体碰到我们的碰撞器时，就给他一个向前的力
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
