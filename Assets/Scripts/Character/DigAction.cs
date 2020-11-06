using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;
namespace MD.Character
{
    [RequireComponent(typeof(ThrowAction))]
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 1;
        
        [SerializeField]
        private GameObject bombPrefab = null;

        private ThrowAction throwAction;

        private IMapManager mapManager = null;
        private IMapManager MapManager
        {
            get
            {
                if (mapManager != null) return mapManager;
                ServiceLocator.Resolve<IMapManager>(out mapManager);
                return mapManager;
            }
        }

        public int Power { get => power; }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            throwAction = GetComponent<ThrowAction>();
            EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(BindAndHoldProjectile);
            EventSystems.EventManager.Instance.StartListening<DigInvokeData>(CmdDig);
        }
        private void OnDestroy()
        {
            if (!isLocalPlayer) return;
            EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(BindAndHoldProjectile);
        }

        public void BindAndHoldProjectile(ProjectileObtainData data)
        {
            throwAction.BindProjectile(Instantiate(bombPrefab, gameObject.transform));
        }

        [Command]
        public void CmdDig(DigInvokeData data)
        {
            Debug.Log("Digging");
            MapManager.DigAtPosition(netIdentity);
        }
    }
}