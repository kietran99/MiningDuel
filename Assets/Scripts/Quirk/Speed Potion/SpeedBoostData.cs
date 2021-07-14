namespace MD.Character
{
    public readonly struct SpeedBoostData : EventSystems.IEventData
    {
        public readonly uint userId;
        public readonly float time;

        public SpeedBoostData(uint userId, float time)
        {
            this.userId = userId;
            this.time = time;
        }
    }
}
