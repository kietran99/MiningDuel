namespace MD.Character
{
    public class DigRequestData : EventSystems.IEventData
    {
        public int x, y, power;

        public DigRequestData(int x, int y, int power)
        {
            this.x = x;
            this.y = y;
            this.power = power;
        }
    }
}