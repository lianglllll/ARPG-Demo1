using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyARPG.Combat
{
    public class EnemyCombatController : NCharacterCombatBase
    {
        //ai�Ĺ���ָ������ai������ָ�ɵģ��������������Ϊ
        //ai���յ�����ָ���Ҫ�ж����������������Ƿ�������ָ��
        //��Ҳ�ϣ��aiȥ���գ�����˵������ڴ�������
        [SerializeField]private bool _attackCommand = false;        //����ָ��
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
        /// ��ȡAI����ָ��״̬
        /// </summary>
        /// <returns></returns>
        public bool GetAttackCommand() => _attackCommand;

        /// <summary>
        /// event,���ù���ָ��
        /// </summary>
        /// <param name="command"></param>
        public void SetAttackCommand(bool command,Transform target = null)
        {
            //�ж���������Ƿ�������ָ��
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
        /// ��⵱ǰai����������Ƿ�������ܹ���ָ��
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
        /// ���õ�ǰai�Ĺ���ָ��
        /// </summary>
        private void ResetAttackCommand()
        {
            _attackCommand = false;
        }

        /// <summary>
        /// ֹͣ������Ϊ
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
        /// ai������������
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
        /// ���������¼��ص�
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
