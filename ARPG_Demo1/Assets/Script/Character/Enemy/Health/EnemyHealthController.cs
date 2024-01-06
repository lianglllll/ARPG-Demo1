using GGG.Tool;
using MyARPG.ComboData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MyARPG.Health
{
    public class EnemyHealthController : CharacterHealthBase
    {
        protected override void Awake()
        {
            base.Awake();
            EnemyManager.Instance.AddEnemyUnit(gameObject);             //���Լ���ӵ�enemymanager��
        }

        /// <summary>
        /// ����ͨ����ʱ�Ļص�
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hitName"></param>
        /// <param name="parrName"></param>
        protected override void CharacterHitAction(float damage,string hitName, string parrName, DamagedType type)
        {

            //1.���жϽ�ɫ������ֵ�Ƿ����0������0��Ӧ�ø񵲶�����ֱ������
            //2.����˺�ֵ����30�����贫�������˺�ֵ����30����Ĭ������һ���Ʒ���������ô��۳�����������ֵ
            if(_characterHealthData.StrengthFull && damage < 30)
            {
                
                if (!_animator.AnimationAtTag("Attack"))//������ڹ��������У������
                {
                    //˵�������Ʒ�����,��ô����Ҫ���и񵲻�������
                    _animator.Play(parrName, 0, 0f);
                    //������Ч
                    GamePoolManager.Instance.TryGetPoolItem("BlockSound", transform.position, Quaternion.identity);
                    //������
                    TakeStrength(damage);
                }

            }
            else
            {
                //���Ű��ᶯ��
                _animator.Play(hitName, 0, 0f);
                //������Ч
                PlayHitClip(type);
                //��Ѫ�߼�
                TakeDamage(damage);
                //����������Ч
                _fx.Play();             

            }
        }

        /// <summary>
        /// �����ˣ���Ѫ
        /// </summary>
        /// <param name="damage"></param>
        protected override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            
            if (CharacterIsDeath())
            {
                GameEventManager.Instance.CallEvent<Transform>("EnemyDeath", transform);            //�������������¼���֪ͨ���
                _animator.SetBool(AnimationID.DeadID, true);
                if(_animator.AnimationAtTag("FinishHit") || _animator.AnimationAtTag("AssassinateHit"))
                {

                }
                else
                {
                    PlayDeadAnimation();
                }
                EnemyManager.Instance.RemoveActiveEnemyUnit(gameObject);                                     //֪ͨenemyManager
            }
        }

    }
}


