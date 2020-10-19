using Timer;
using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : MonoBehaviour, Timer.ITickListener
    {
        [SerializeField]
        private ProjectileStats stats = null;

        [SerializeField]
        private bool isDug = false;

        private ITimer timer = null;

        void Start()
        {
            timer = GetComponent<ITimer>();
        }

        void Update()
        {
            if (isDug && Input.GetKeyDown(KeyCode.A)) timer.Activate();
        }

        public void OnTick()
        {
            Debug.Log("Explode!");            
        }
    }
}