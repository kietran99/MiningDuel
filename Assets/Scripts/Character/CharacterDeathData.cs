namespace MD.Character
{
    public struct CharacterDeathData : EventSystems.IEventData
    {
        public uint eliminatedId;

        public CharacterDeathData(uint eliminatedId)
        {
            this.eliminatedId = eliminatedId;
        }
    }
}