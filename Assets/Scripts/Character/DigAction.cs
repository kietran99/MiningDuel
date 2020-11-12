using UnityEngine;
using Mirror;

namespace MD.Character
{
    [RequireComponent(typeof(Player))]
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 1;

        private float digTime = .1f;
        private bool isDigging = false;

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
            EventSystems.EventManager.Instance.StartListening<DigInvokeData>(Dig);
        }

        private void OnDestroy()
        {
            if (!isLocalPlayer) return;
            // EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(BindAndHoldProjectile);
            EventSystems.EventManager.Instance.StartListening<DigInvokeData>(Dig);
        }

        // public void BindAndHoldProjectile(ProjectileObtainData data)
        // {
        //     throwAction.BindProjectile(Instantiate(bombPrefab, gameObject.transform));
        // }

        private void Dig(DigInvokeData data)
        {
            if (isDigging) return;
            isDigging = true;
            CmdDig();
            Invoke(nameof(EnableCanDig),digTime);
        }
        private void EnableCanDig()
        {
            isDigging = false;
        }

        [Command]
        public void CmdDig()
        {
            Player.SetCanMove(false);
            MapManager.DigAtPosition(netIdentity);
            Invoke(nameof(EnableCanMove),digTime);
        }
        [Server]
        public void EnableCanMove()
        {
            Player.SetCanMove(true);
        }

    }
}