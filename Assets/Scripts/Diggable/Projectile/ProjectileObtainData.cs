using Mirror;

namespace MD.Diggable.Projectile
{
    public class ProjectileObtainData : EventSystems.IEventData
    {
        public NetworkIdentity thrower;
        public DiggableType type;

        public ProjectileObtainData() {}

        public ProjectileObtainData(NetworkIdentity thrower, DiggableType type)
        {
            this.thrower = thrower;
            this.type = type;
        }
    }
}