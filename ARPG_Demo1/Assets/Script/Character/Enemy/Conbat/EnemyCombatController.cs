using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Combat
{
    public class EnemyCombatController : NCharacterCombatBase
    {
        //ai的攻击指令是由ai管理器指派的，并不是自身的行为
        //ai在收到攻击指令，需要判断自身的情况来决定是否接收这个指令
        //玩家不希望ai去接收，比如说玩家正在处决敌人
        [SerializeField]private bool _attackCommand = false;        //攻击指令
        private int maxAttackNumber = 0;
        private int _currentAttackNumber;

        private void OnEnable()
        {
            GameEventManager.Instance.AddEventListening<Transform>("EnemyDeath", EnemyDeathHandler);

        }

        private void OnDisable()
        {
            GameEventManager.Instance.RemoveEvent<Transform>("EnemyDeath", EnemyDeathHandler);
        }
        protected override void Start()
        {
            base.Start();
            maxAttackNumber = (int)DefaultComboData.ActionColdTime;
            _currentAttackNumber = 0;
        }

        /// <summary>
        /// 获取AI攻击指令状态
        /// </summary>
        /// <returns></returns>
        public bool GetAttackCommand() => _attackCommand;

        /// <summary>
        /// event,设置攻击指令
        /// </summary>
        /// <param name="command"></param>
        public void SetAttackCommand(bool command,Transform target = null)
        {
            //判断自身情况是否接受这个指令
            if (!CheckAIState())
            {
                ResetAttackCommand();
                return;
            }
            _currentEnemy = target;
            _attackCommand = command;
            _currentAttackNumber = 0;
        }

        /// <summary>
        /// 检测当前ai的自身情况是否允许接受攻击指令
        /// </summary>
        /// <returns></returns>
        private bool CheckAIState()
        {
            if (_animator.AnimationAtTag("Hit")) return false;
            if (_animator.AnimationAtTag("Parry")) return false;
            if (_animator.AnimationAtTag("Finish")) return false;
            if (_animator.AnimationAtTag("Attack")) return false;
            if (_attackCommand) return false;
            return true;
        }

        /// <summary>
        /// 重置当前ai的攻击指令
        /// </summary>
        private void ResetAttackCommand()
        {
            _attackCommand = false;
        }

        /// <summary>
        /// 停止攻击行为
        /// </summary>
        public void StopAllAction()
        {
            if (_attackCommand)
            {
                ResetAttackCommand();
            }
            if (_animator.AnimationAtTag("Attack"))
            {
                _animator.Play("Idle",0,0f);
            }
        }

        /// <summary>
        /// ai基础攻击输入
        /// </summary>
        public void AIBaseAttackInput()
        {
            if (!_applyAttackInput) return;
            if (_currentAttackNumber >= maxAttackNumber)
            {
                ResetAttackCommand();
                return;
            }
            _currentAttackNumber++;
            UpdateDateComboData();
            ComboActionExecute();
        }

        /// <summary>
        /// 敌人死亡事件回调
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyDeathHandler(Transform enemy)
        {
            if(transform == enemy)
            {
                ResetAttackCommand();
                ResetComboData();
                _applyAttackInput = true;
            }
        }

        public void UpdateDateComboData()
        {
            _currentComboData = _currentComboData.NextComboData;
        }

    }

}
