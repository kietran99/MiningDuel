namespace MD.Character
{
    public class ScoreChangeData : EventSystems.IEventData
    {
        public int newScore;

        public ScoreChangeData(int newScore)
        {
            this.newScore = newScore;
        }
    }
}
