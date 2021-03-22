using System.Collections;
using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialScanWave : MonoBehaviour
    {
        [SerializeField]
        private float maxExistDuration = 20f;

        [SerializeField]
        private float durationAfterSpread = 3f;

        [SerializeField]
        private float spreadRange = 7f;

        [SerializeField]
        private float spreadSpeed = .3f;
        
        [SerializeField]
        private float smoothness = .04f;

        private SpriteRenderer waveSprite;
        private SpriteMask spriteMask;
        private float BASE_SCALE = .25f;

        void Start()
        {
            waveSprite = GetComponent<SpriteRenderer>();
            spriteMask = GetComponent<SpriteMask>();
            spriteMask.enabled = true;
            waveSprite.material.SetColor("_Color", new Color(waveSprite.color.r, waveSprite.color.g, waveSprite.color.b, .5f));
            StartCoroutine(nameof(Spread));
            Invoke(nameof(Destroy), maxExistDuration);
        }

        private IEnumerator Spread()
        {
            float currentRange = 1f;
            WaitForSeconds delayWFS = new WaitForSeconds(smoothness);

            while(currentRange < spreadRange)
            {
                yield return delayWFS;
                currentRange += (1/currentRange)*spreadSpeed;
                if (currentRange > spreadRange) currentRange = spreadRange;
                transform.localScale = new Vector3(currentRange*BASE_SCALE,currentRange*BASE_SCALE,1f);
            }

            Invoke(nameof(Destroy), durationAfterSpread);
        }

        private void Destroy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
