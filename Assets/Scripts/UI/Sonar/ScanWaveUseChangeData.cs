public class ScanWaveChangeData: EventSystems.IEventData
{
    public int maxLevel;
    public int currentLevel;

    public ScanWaveChangeData(int currentLevel, int maxLevel)
    {
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
    }
}