using GGG.Tool;
using MyARPG.ComboData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyARPG.Health
{

    public class PlayerHealthController : CharacterHealthBase
    {

        protected override void Update()
        {
            base.Update();
            PlayerParryInput();
        }
        protected override void CharacterHitAction(float damage, string hitName, string parrName,DamagedType type)
        {
            if (_animator.AnimationAtTag("Finish")) return;//玩家在处决的时候，不接受伤害信息

            //当玩家按着格挡键 && 当前技能不是一个破防技能
            if (_animator.GetBool(AnimationID.ParryID) && damage < 30f)
            {
                //播放格挡动画
                _animator.Play(parrName, 0, 0f);
                GamePoolManager.Instance.TryGetPoolItem("BlockSound", transform.position, Quaternion.identity);
                TakeStrength(damage);            
            }
            else
            {
                _animator.Play(hitName, 0, 0f);
                GamePoolManager.Instance.TryGetPoolItem("HitSound", transform.position, Quaternion.identity);
                TakeDamage(damage);
            }
        }


        //格挡输入
        private void PlayerParryInput()
        {
            if (_animator.AnimationAtTag("Hit") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.35f) return;
            if (_animator.AnimationAtTag("FinishHit")) return;
            _animator.SetBool(AnimationID.ParryID, InputManager.Instance.Parry);
        }


    }

}
