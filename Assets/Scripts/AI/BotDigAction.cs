using UnityEngine;
using Mirror;

namespace MD.AI
{
    public class BotDigAction : MD.Character.DigAction
    {   
        [SerializeField]
        PlayerBot bot = null;

        protected override bool IsPlayer => false;

        void Start()
        {
            bot = GetComponent<PlayerBot>();
        }
        protected override void StartListeningToEvents()
        {
            EventSystems.EventManager.Instance.StartListening<BotDigAnimEndData>(NotifyEndDig);
        }

        protected override void StopListeningToEvents()
        {
            EventSystems.EventManager.Instance.StopListening<BotDigAnimEndData>(NotifyEndDig);
        }

        private void NotifyEndDig(BotDigAnimEndData data)
        {
            if (bot != null)
            {
                bot.isDigging = false;
                CmdDig(power);
            }
        }

        [Command]
        protected override void CmdDig(int power)
        {
            ServiceLocator
                .Resolve<Diggable.Core.IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    diggableGenerator => 
                        diggableGenerator.BotDigAt(
                            bot, 
                            Mathf.FloorToInt(transform.position.x), 
                            Mathf.FloorToInt(transform.position.y), 
                            power)                
                );
        }
    }
}
