namespace MD.Character
{
    public class ThrowRaycastHitData : EventSystems.IEventData
    {
        public UnityEngine.Transform hitTransform;

        public ThrowRaycastHitData(UnityEngine.Transform hitTransform)
        {
            this.hitTransform = hitTransform;
        }
    }
}