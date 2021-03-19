namespace MD.Character
{
    public struct FinalScoreChangeData : EventSystems.IEventData
    {
        public int finalScore;

        public FinalScoreChangeData(int finalScore)
        {
            this.finalScore = finalScore;
        }
    }
}