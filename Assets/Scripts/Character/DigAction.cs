using UnityEngine;
using Mirror;
using MD.Character.Animation;
using MD.Map.Core;

namespace MD.Character
{
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 1;

        #region FIELDS
        private IMapManager mapManager = null;
        private Player player = null;
        #endregion

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

        public override void OnStartAuthority()
        {
            StartListeningToEvents();
        }

        private void OnDestroy()
        {
            StopListeningToEvents();
        }

        protected virtual void StartListeningToEvents()
        {
            EventSystems.EventManager.Instance.StartListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected virtual void StopListeningToEvents()
        {
            EventSystems.EventManager.Instance.StopListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected void HandleDigAnimEnd(DigAnimEndData data) => CmdDig();

        [Command]
        public void CmdDig()
        {
            if (Player != null)
            {
                Player.Movable(false);
                EnableMovement();
            }
       
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailService => Debug.LogError(UnavailableServiceError.MESSAGE),
                    digGen => digGen.DigAt(
                                Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y),
                                power, netId)
                );  
            MapManager.DigAt(netIdentity, transform.position);         
            // EventSystems.EventManager.Instance.TriggerEvent(
            //     new DigRequestData(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), power));
        }
        
        [Server]
        public void EnableMovement()
        {
            Player.Movable(true);
        }
    }
}