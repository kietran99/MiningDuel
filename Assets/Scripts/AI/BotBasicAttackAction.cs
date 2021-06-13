using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MD.AI
{
    public class BotBasicAttackAction : MD.Character.BasicAttackAction
    {
        public override void OnStartAuthority()
        {
            // base.OnStartAuthority();
        }

        public void Attack()
        {
            enemyDetect.RaiseAttackDirEvent();
            pickaxeAnimatorController.Play();
            CmdAttemptSwingWeapon();
        }
    }
}