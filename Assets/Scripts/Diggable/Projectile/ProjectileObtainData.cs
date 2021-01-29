namespace MD.Diggable.Projectile
{
    public class ProjectileObtainData : EventSystems.IEventData
    {
        public ProjectileStats stats;
        public float posX;
        public float posY;

        public ProjectileObtainData(ProjectileStats stats, float posX, float posY)
        {
            this.stats = stats;
            this.posX = posX;
            this.posY = posY;
        }
    }
}