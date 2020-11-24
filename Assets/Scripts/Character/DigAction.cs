using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 1;

        private float digCooldown = .1f;
        private float nextDigTime = 0f;
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

        private Player player = null;
        private Player Player
        {
            get
            {
                if (player != null) return player;
                return player = GetComponent<Player>();
            }
        }

        public int Power { get => power; }

        public override void OnStartAuthority()
        {
            // throwAction = GetComponent<ThrowAction>();
            // EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(BindAndHoldProjectile);
            if(isLocalPlayer)
                EventSystems.EventManager.Instance.StartListening<DigInvokeData>(Dig);
        }

        private void OnDestroy()
        {
            if (!isLocalPlayer) return;
            // EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(BindAndHoldProjectile);
            StopAllCoroutines();
            EventSystems.EventManager.Instance.StopListening<DigInvokeData>(Dig);
        }

        // public void BindAndHoldProjectile(ProjectileObtainData data)
        // {
        //     throwAction.BindProjectile(Instantiate(bombPrefab, gameObject.transform));
        // }

        private void Dig(DigInvokeData data)
        {
            if (Time.time < nextDigTime) return;

            nextDigTime = Time.time + digCooldown;
            CmdDig();
        }

        [Command]
        public void CmdDig()
        {
            if (Player != null)
            {
                Player.SetCanMove(false);
                Invoke(nameof(EnableCanMove),digCooldown);
            }
            MapManager.DigAtPosition(netIdentity);

        }
        
        [Server]
        public void EnableCanMove()
        {
            Player.SetCanMove(true);
        }

    }
}