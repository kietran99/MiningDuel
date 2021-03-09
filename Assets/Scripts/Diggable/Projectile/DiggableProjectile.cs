using UnityEngine;

namespace MD.Diggable.Projectile
{
    public class DiggableProjectile : MonoBehaviour
    {
        [SerializeField]
        private int projectileType = -1;
        
        [SerializeField]
        private int value = 1;

        public int RemainingHit { get; private set; }
        
        private void Start()
        {
            RemainingHit = value;               
        }
        
        public int Type => projectileType;
    }
}