public class BotDigAction : MD.Character.DigAction
{   
    protected override void StartListeningToEvents()
    {
        EventSystems.EventManager.Instance.StartListening<BotDigAnimEndData>(Dig);
    }

    protected override void StopListeningToEvents()
    {
        EventSystems.EventManager.Instance.StopListening<BotDigAnimEndData>(Dig);
    }

    private void Dig(BotDigAnimEndData data) => Dig();
}
