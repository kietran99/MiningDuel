public struct EndGameData: EventSystems.IEventData
{
    public int score;

    public EndGameData(int score)
    {
        this.score = score;
    }
}