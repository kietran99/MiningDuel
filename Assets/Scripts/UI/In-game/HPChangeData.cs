namespace MD.Character
{
    public struct HPChangeData: EventSystems.IEventData
    {
        public int lastHP, curHP, maxHP;

        public HPChangeData(int lastHP, int curHP, int maxHP)
        {
            this.lastHP = lastHP;
            this.curHP = curHP;
            this.maxHP = maxHP;
        }
    }
}
