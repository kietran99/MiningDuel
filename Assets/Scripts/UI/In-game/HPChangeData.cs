namespace MD.Character
{
    public struct HPChangeData: EventSystems.IEventData
    {
        public int curHP, maxHP;

        public HPChangeData(int curHP, int maxHP)
        {
            this.curHP = curHP;
            this.maxHP = maxHP;
        }
    }
}
