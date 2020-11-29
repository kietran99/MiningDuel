using UnityEngine;
using Mirror;
using MD.Character.Animation;

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
            // if (isLocalPlayer)
            // {
            //     //EventSystems.EventManager.Instance.StartListening<DigInvokeData>(Dig);
            //     StartListeningToEvents();
            // }
            StartListeningToEvents();
        }

        private void OnDestroy()
        {
            StopListeningToEvents();
            //if (!isLocalPlayer) return;
            // EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(BindAndHoldProjectile);
            //EventSystems.EventManager.Instance.StopListening<DigInvokeData>(Dig);
            //StopListeningToEvents();
        }

        // public void BindAndHoldProjectile(ProjectileObtainData data)
        // {
        //     throwAction.BindProjectile(Instantiate(bombPrefab, gameObject.transform));
        // }

        protected virtual void StartListeningToEvents()
        {
            EventSystems.EventManager.Instance.StartListening<DigAnimEndData>(Dig);
        }

        protected virtual void StopListeningToEvents()
        {
            EventSystems.EventManager.Instance.StopListening<DigAnimEndData>(Dig);
        }

        protected void Dig(DigAnimEndData data)
        {
            Dig();
        }

        private void Dig(DigInvokeData data)
        {
            Dig();
        }

        protected void Dig()
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
                Invoke(nameof(EnableCanMove), digCooldown);
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