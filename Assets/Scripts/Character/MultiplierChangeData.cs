namespace MD.Character
{
    public struct MultiplierChangeData : EventSystems.IEventData
    {
        public float newMult;

        public MultiplierChangeData(float newMult)
        {
            this.newMult = newMult;
        }
    }
}    