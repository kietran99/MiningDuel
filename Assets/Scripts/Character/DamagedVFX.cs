using UnityEngine;

namespace MD.VisualEffects
{
    public class DamagedVFX : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer playerRenderer = null;

        [SerializeField]
        private int numberOfFlashes = 3;

        [SerializeField]
        private float flashDelay = .075f;

        private Color originalPlayerColor;
        private WaitForSecondsRealtime flashDelayAsWaitForSeconds;

        void Start()
        {
            originalPlayerColor = playerRenderer.color;
            flashDelayAsWaitForSeconds = new WaitForSecondsRealtime(flashDelay);
        }

        // [ClientRpc]
        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(PlayDamagingEffect());
        }

        // [Client]
        private System.Collections.IEnumerator PlayDamagingEffect()
        {
            int flashCnt = 0;

            while (flashCnt < numberOfFlashes)
            {               
                playerRenderer.color = Color.red;
                yield return flashDelayAsWaitForSeconds;
                playerRenderer.color = originalPlayerColor;
                flashCnt++;

                yield return flashDelayAsWaitForSeconds;
            }
        }
    }
}
