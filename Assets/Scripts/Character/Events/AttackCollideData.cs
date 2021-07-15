namespace MD.Character
{
    public struct AttackCollideData : EventSystems.IEventData
    {
        public float posX, posY;

        public AttackCollideData(float posX, float posY)
        {
            this.posX = posX;
            this.posY = posY;
        }
    }
}

