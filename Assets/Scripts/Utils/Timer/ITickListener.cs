namespace Timer
{
    public interface ITickListener
    {
        void OnTick(float timeStamp);
    }
}