namespace MD.UI
{     
    public struct MenuSwitchEvent: EventSystems.IEventData
    {
        public bool switchToInventoryMenu;
        public MenuSwitchEvent(bool switchToInventoryMenu)
        {
            this.switchToInventoryMenu = switchToInventoryMenu;
        }
    }
}
