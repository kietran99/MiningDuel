using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[RequireComponent(typeof(SpriteRenderer))]
public class ScanWave : NetworkBehaviour
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
        if (hasAuthority)
        {
            InitializeWithAuthority();
        }
        else
        {
            InitializeWithoutAuthority();
        }
        StartCoroutine(nameof(Spread));
        Invoke(nameof(Destroy),maxExistDuration);
    }

    void Destroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    void InitializeWithAuthority()
    {
        spriteMask.enabled = true;
        waveSprite.material.SetColor("_Color",new Color(waveSprite.color.r,waveSprite.color.g,waveSprite.color.b,.5f));
    }
    void InitializeWithoutAuthority()
    {
        spriteMask.enabled = false;
        waveSprite.material.SetColor("_Color",new Color(waveSprite.color.r,waveSprite.color.g,waveSprite.color.b,1f));
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
        Invoke(nameof(Destroy),durationAfterSpread);
    }
}
