using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : MonoBehaviour, Timer.ITickListener
    {
        [SerializeField]
        private ProjectileStats stats = null;

        void Start()
        {
            
        }

        void Update()
        {

        }

        public void OnTick()
        {
            Debug.Log("Explode!");            
        }
    }
}