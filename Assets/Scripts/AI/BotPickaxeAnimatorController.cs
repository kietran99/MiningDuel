using System.Collections;
using System.Collections.Generic;
using MD.Character;
using UnityEngine;
namespace MD.AI
{
    public class BotPickaxeAnimatorController : MD.Character.PickaxeAnimatorController
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<BotGetCounteredData>(CancelAnim);
        }

        private void CancelAnim(BotGetCounteredData _)
        {
            Debug.Log("bot get countered");
            animator.SetTrigger(INTERRUPT);
        }
    }
}