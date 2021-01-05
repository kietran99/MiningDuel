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
            RemainingHit = value;   
             
            if (!isClient) return;
            
            if (ServiceLocator.Resolve<IMapManager>(out IMapManager mapManager))
            {
                mapManager.NotifyNewGem(transform.position,value.ToDiggable());
            }           
        }

        public void DecreaseValue(int power, out bool obtainable)
        {
            var decAmt = RemainingHit - power;
            RemainingHit = decAmt > 0 ? decAmt : 0;
            obtainable = RemainingHit == 0;
        }
    }
}