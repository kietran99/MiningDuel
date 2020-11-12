public struct EndGameData: EventSystems.IEventData
{
    public int score;
    public bool hasWon;
    public EndGameData(bool hasWon, int score)
    {
        this.score = score;
        this.hasWon = hasWon;
    }
}