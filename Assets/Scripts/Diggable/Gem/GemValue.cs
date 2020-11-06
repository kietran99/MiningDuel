using UnityEngine;
using Mirror;
namespace MD.Diggable.Gem
{
    public class GemValue : NetworkBehaviour
    {
        [SerializeField]
        private int value = 1;

        public int RemainingHit { get; private set; }

        public int Value { get => value; set => this.value = value; }

        private void Start()
        {
            Debug.Log("spawned gem");
            RemainingHit = value;    
            if (isClient)
            {
                Debug.Log("spawned gem is client");
                IMapManager mapManager;
                if (ServiceLocator.Resolve<IMapManager>(out mapManager))
                {
                    mapManager.NotifyNewGem(transform.position,value);
                    Debug.Log("spawned gem finished");
                }
            }
        }

        public void DecreaseValue(int power)
        {
            var decAmt = RemainingHit - power;
            RemainingHit = decAmt > 0 ? decAmt : 0;
        }
    }
}