using UnityEngine;

namespace MD.Diggable.Projectile
{
    public class ProjectilePickupData : EventSystems.IEventData
    {
        public Sprite sprite;

        public ProjectilePickupData(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}