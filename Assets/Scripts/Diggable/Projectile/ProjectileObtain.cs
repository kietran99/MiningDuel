using UnityEngine;
using Mirror;
using MD.Character;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(DiggableProjectile))]
    public class ProjectileObtain : NetworkBehaviour, ICanDig
    {
        // private bool diggable = false;
        private DigAction currentDigger = null;
        
        private DiggableProjectile projectile = null;
        private DiggableProjectile Projectile
        {
            get
            {
                if (projectile != null) return projectile;
                return projectile = GetComponent<DiggableProjectile>();
            }
        }  
        public override void OnStartClient()
        {
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableSpawnData(Projectile.GetStats().DigValue,transform.position.x,transform.position.y));
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            //fire an event for sonar to update
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableDestroyData(Projectile.GetStats().DigValue,transform.position.x,transform.position.y));
            //for animations and UI
            EventSystems.EventManager.Instance.TriggerEvent(
                new ProjectileObtainData(Projectile.GetStats(),transform.position.x,transform.position.y)
            );
        }

        [Server]
        public void Dig(DigAction digger)
        {
            currentDigger = digger;
            EventSystems.EventManager.Instance.TriggerEvent(
                new ServerDiggableDestroyData(Projectile.DiggbleType(),transform.position.x,transform.position.y, currentDigger));
            Destroy(gameObject);
        }

    }
}