namespace MD.Character
{
    public struct AttackCollideData : EventSystems.IEventData
    {
        public float posX, posY;
        public bool isCritical;

        public AttackCollideData(float posX, float posY, bool isCritical)
        {
            this.posX = posX;
            this.posY = posY;
            this.isCritical = isCritical;
        }
    }
}

