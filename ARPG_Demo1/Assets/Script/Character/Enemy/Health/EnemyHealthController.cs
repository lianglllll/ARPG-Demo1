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
            EnemyManager.Instance.AddEnemyUnit(gameObject);             //将自己添加到enemymanager中
        }

        /// <summary>
        /// 被普通攻击时的回调
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hitName"></param>
        /// <param name="parrName"></param>
        protected override void CharacterHitAction(float damage,string hitName, string parrName, DamagedType type)
        {

            //1.先判断角色的体力值是否大于0，大于0就应该格挡而不是直接受伤
            //2.如果伤害值大于30（假设传进来的伤害值大于30），默认这是一个破防攻击，那么会扣除大量的体力值
            if(_characterHealthData.StrengthFull && damage < 30)
            {
                
                if (!_animator.AnimationAtTag("Attack"))//如果不在攻击动作中，允许格挡
                {
                    //说明不是破防动作,那么我们要进行格挡或者闪避
                    _animator.Play(parrName, 0, 0f);
                    //播放音效
                    GamePoolManager.Instance.TryGetPoolItem("BlockSound", transform.position, Quaternion.identity);
                    //扣体力
                    TakeStrength(damage);
                }

            }
            else
            {
                //播放挨揍动画
                _animator.Play(hitName, 0, 0f);
                //播放音效
                PlayHitClip(type);
                //扣血逻辑
                TakeDamage(damage);
                //播放粒子特效
                _fx.Play();             

            }
        }

        /// <summary>
        /// 被打了，扣血
        /// </summary>
        /// <param name="damage"></param>
        protected override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            
            if (CharacterIsDeath())
            {
                GameEventManager.Instance.CallEvent<Transform>("EnemyDeath", transform);            //触发敌人死亡事件，通知玩家
                _animator.SetBool(AnimationID.DeadID, true);
                if(_animator.AnimationAtTag("FinishHit") || _animator.AnimationAtTag("AssassinateHit"))
                {

                }
                else
                {
                    PlayDeadAnimation();
                }
                EnemyManager.Instance.RemoveActiveEnemyUnit(gameObject);                                     //通知enemyManager
            }
        }

    }
}


