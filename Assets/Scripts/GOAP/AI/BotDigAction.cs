public class BotDigAction : MD.Character.DigAction
{   
    PlayerBot bot;
    protected override void Start()
    {
        base.Start();
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
        if (bot) bot.NotifyDigComplete();
    }
}
