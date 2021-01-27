using UnityEngine;
public class BotDigAction : MD.Character.DigAction
{   
    [SerializeField]
    PlayerBot bot;
    void Start()
    {
        bot = GetComponent<PlayerBot>();
    }
    protected override void ListenToEvents()
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
            CmdDig();
        }
    }
}
