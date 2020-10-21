using MD.Diggable.Projectile;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [SerializeField]
    private Sprite idleSprite = null, projectileHoldSprite = null, digSprite = null;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(SwapProjHoldSprite);
        EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(SwapIdleSprite);
        EventSystems.EventManager.Instance.StartListening<ExplodeData>(SwapIdleSprite);
        EventSystems.EventManager.Instance.StartListening<DigInvokeData>(SwapDigSprite);
    }

    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(SwapProjHoldSprite);
        EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(SwapIdleSprite);
        EventSystems.EventManager.Instance.StopListening<ExplodeData>(SwapIdleSprite);
        EventSystems.EventManager.Instance.StopListening<DigInvokeData>(SwapDigSprite);
    }

    private void SwapDigSprite(DigInvokeData obj)
    {
        spriteRenderer.sprite = digSprite;
        Invoke(nameof(SwapIdleSprite), .2f);
    }

    public void SwapProjHoldSprite(ProjectileObtainData obj)
    {
        spriteRenderer.sprite = projectileHoldSprite;
    }

    private void SwapIdleSprite(ThrowInvokeData obj) => SwapIdleSprite();

    private void SwapIdleSprite(ExplodeData obj) => SwapIdleSprite();

    private void SwapIdleSprite()
    {
        spriteRenderer.sprite = idleSprite;
    }
}
