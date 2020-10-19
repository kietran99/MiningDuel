namespace MD.Diggable.Projectile
{
    public class ProjectileObtainData : EventSystems.IEventData
    {
        public ProjectileStats stats;

        public ProjectileObtainData(ProjectileStats stats)
        {
            this.stats = stats;
        }
    }
}