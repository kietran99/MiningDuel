using UnityEngine;
using Mirror;
using MD.Character.Animation;

namespace MD.Character
{
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 1;

        #region FIELDS
        private float digCooldown = .1f;
        private float nextDigTime = 0f;
        private IMapManager mapManager = null;
        private Player player = null;
        #endregion

        #region  PROPERTIES
        private IMapManager MapManager
        {
            get
            {
                if (mapManager != null) return mapManager;
                ServiceLocator.Resolve<IMapManager>(out mapManager);
                return mapManager;
            }
        }

        private Player Player
        {
            get
            {
                if (player != null) return player;
                return player = GetComponent<Player>();
            }
        }

        public int Power { get => power; }
        #endregion

        public override void OnStartAuthority()
        {
            ListenToEvents();
        }

        private void OnDestroy()
        {
            StopListeningToEvents();
        }

        protected virtual void ListenToEvents()
        {
            EventSystems.EventManager.Instance.StartListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected virtual void StopListeningToEvents()
        {
            EventSystems.EventManager.Instance.StopListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected void HandleDigAnimEnd(DigAnimEndData data) => Dig();
       
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

            MapManager.DigAt(netIdentity, transform.position);
        }
        
        [Server]
        public void EnableCanMove()
        {
            Player.SetCanMove(true);
        }
    }
}