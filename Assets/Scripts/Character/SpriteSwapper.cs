using MD.Diggable;
using MD.Diggable.Projectile;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [SerializeField]
    private Sprite idleSprite = null, projectileHoldSprite = null, digSprite = null;

    private readonly float SPRITE_TRANSITION_TIME = .2f;

    private SpriteRenderer spriteRenderer;

    private bool hasDugGem;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var eventManager = EventSystems.EventManager.Instance;
        eventManager.StartListening<ProjectileObtainData>(SwapProjHoldSprite);
        eventManager.StartListening<ThrowInvokeData>(SwapIdleSprite);
        eventManager.StartListening<ExplodeData>(SwapIdleSprite);
        eventManager.StartListening<DigInvokeData>(SwapDigSprite);
        eventManager.StartListening<GemDigSuccessData>(SwapIdleSpriteWithDelay);
    }

    void OnDestroy()
    {
        var eventManager = EventSystems.EventManager.Instance;
        eventManager.StopListening<ProjectileObtainData>(SwapProjHoldSprite);
        eventManager.StopListening<ThrowInvokeData>(SwapIdleSprite);
        eventManager.StopListening<ExplodeData>(SwapIdleSprite);
        eventManager.StopListening<DigInvokeData>(SwapDigSprite);
        eventManager.StopListening<GemDigSuccessData>(SwapIdleSpriteWithDelay);
    }
    
    private void SwapDigSprite(DigInvokeData obj)
    {
        hasDugGem = true;
        spriteRenderer.sprite = digSprite;
        StartCoroutine(RevertToIdleIfNothingDug());
    }

    private IEnumerator RevertToIdleIfNothingDug()
    {
        yield return new WaitForSecondsRealtime(SPRITE_TRANSITION_TIME);
        yield return null;
        if (hasDugGem) SwapIdleSprite();
    }

    private void SwapIdleSpriteWithDelay(GemDigSuccessData obj)
    {
        Invoke(nameof(SwapIdleSprite), SPRITE_TRANSITION_TIME);
    }

    public void SwapProjHoldSprite(ProjectileObtainData obj)
    {
        spriteRenderer.sprite = projectileHoldSprite;
        hasDugGem = false;
    }

    private void SwapIdleSprite(ThrowInvokeData obj) => SwapIdleSprite();

    private void SwapIdleSprite(ExplodeData obj) => SwapIdleSprite();

    private void SwapIdleSprite()
    {
        spriteRenderer.sprite = idleSprite;
    }
}
