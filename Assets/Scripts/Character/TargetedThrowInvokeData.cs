namespace MD.Character
{
    public struct TargetedThrowInvokeData : EventSystems.IEventData
    {
        public float x, y;

        public TargetedThrowInvokeData(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}