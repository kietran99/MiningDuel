using UnityEngine;

namespace MD.Diggable.Gem
{
    public class GemValue : MonoBehaviour
    {
        [SerializeField]
        private int value = 1;

        public int RemainingHit { get; private set; }

        public int Value { get => value; set => this.value = value; }

        private void Start()
        {
            RemainingHit = value;       
        }

        public void DecreaseValue(int power, out bool obtainable)
        {
            var decAmt = RemainingHit - power;
            RemainingHit = decAmt > 0 ? decAmt : 0;
            obtainable = RemainingHit == 0;
        }
    }
}