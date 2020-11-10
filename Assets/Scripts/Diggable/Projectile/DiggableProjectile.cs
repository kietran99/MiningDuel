using UnityEngine;
using Mirror;
namespace MD.Diggable.Projectile
{
    public class DiggableProjectile : NetworkBehaviour
    {
        [SerializeField]
        private int projectileType = -1;
        
        [SerializeField]
        private int value = 1;
        [SerializeField]
        private ProjectileStats stats = null;

        public int RemainingHit { get; private set; }

        public int Value { get => value; set => this.value = value; }

        public ProjectileStats GetStats()
        {
            return stats;
        }
        private void Start()
        {
            RemainingHit = value;    
            if (isClient)
            {
                IMapManager mapManager;
                if (ServiceLocator.Resolve<IMapManager>(out mapManager))
                {
                    mapManager.NotifyNewGem(transform.position,projectileType.ToDiggable());
                }
            }
        }
        public int DiggbleType() => projectileType;

        public void DecreaseValue(int power)
        {
            var decAmt = RemainingHit - power;
            RemainingHit = decAmt > 0 ? decAmt : 0;
        }
    }
}