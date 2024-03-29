﻿using System.Collections;
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
        private CircleCollider2D theCollider;
        private float BASE_SCALE = .25f;
        private float BASE_COLLIDER_RADIUS = 2;

        void Start()
        {
            waveSprite = GetComponent<SpriteRenderer>();
            spriteMask = GetComponent<SpriteMask>();
            theCollider = GetComponent<CircleCollider2D>();
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
                currentRange += (1 / currentRange) * spreadSpeed;
                if (currentRange > spreadRange) currentRange = spreadRange;
                transform.localScale = new Vector3(currentRange * BASE_SCALE, currentRange * BASE_SCALE, 1f);
                theCollider.radius = BASE_COLLIDER_RADIUS * currentRange;
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
