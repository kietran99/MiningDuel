using Mirror;

namespace MD.Diggable.Projectile
{
    public class ProjectileObtainData : EventSystems.IEventData
    {
        public NetworkIdentity networkIdentity;
        public DiggableType type;

        public ProjectileObtainData() {}

        public ProjectileObtainData(NetworkIdentity networkIdentity, DiggableType type)
        {
            this.networkIdentity = networkIdentity;
            this.type = type;
        }
    }
}