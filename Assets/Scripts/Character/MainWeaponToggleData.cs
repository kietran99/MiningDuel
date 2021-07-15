namespace MD.Character
{
    public struct MainWeaponToggleData: EventSystems.IEventData
    {
        public bool isActive;

        public MainWeaponToggleData(bool isActive)
        {
            this.isActive = isActive;
        }
    }
}