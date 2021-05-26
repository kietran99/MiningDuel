namespace MD.Character
{
    public struct HitScoreObtainData: EventSystems.IEventData
    {
        public int score;

        public HitScoreObtainData(int score)
        {
            this.score = score;
        }
    }
}