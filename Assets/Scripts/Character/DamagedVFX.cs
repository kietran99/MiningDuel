using UnityEngine;
using MD.Character;

namespace MD.VisualEffects
{
    public class DamagedVFX : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints = null;

        [SerializeField]
        private SpriteRenderer playerRenderer = null;

        [SerializeField]
        private int numberOfFlashes = 3;

        [SerializeField]
        private float flashDelay = .075f;

        private Color originalPlayerColor;
        private WaitForSecondsRealtime flashDelayWFS;

        void Start()
        {
            originalPlayerColor = playerRenderer.color;
            flashDelayWFS = new WaitForSecondsRealtime(flashDelay);
            hitPoints.OnDamageTakenSync += Play;
        }

        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(PlayDamagingEffect());
        }

        private System.Collections.IEnumerator PlayDamagingEffect()
        {
            int flashCnt = 0;

            while (flashCnt < numberOfFlashes)
            {               
                playerRenderer.color = Color.red;

                yield return flashDelayWFS;

                playerRenderer.color = originalPlayerColor;
                flashCnt++;

                yield return flashDelayWFS;
            }
        }
    }
}
