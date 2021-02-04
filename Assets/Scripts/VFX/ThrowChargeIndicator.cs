using MD.UI;
using UnityEngine;

namespace MD.Character
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThrowChargeIndicator : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        void Start()
        {           
            spriteRenderer = GetComponent<SpriteRenderer>();    
        }

        public void Show() => spriteRenderer.enabled = true;

        public void Hide() => spriteRenderer.enabled = false;
    }
}
