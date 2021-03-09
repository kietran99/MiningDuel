namespace MD.Character
{
    public struct ScoreChangeData : EventSystems.IEventData
    {
        public int newScore;
        public int finalScore;

        public ScoreChangeData(int newScore, int finalScore)
        {
            this.newScore = newScore;
            this.finalScore = finalScore;
        }
    }
}
