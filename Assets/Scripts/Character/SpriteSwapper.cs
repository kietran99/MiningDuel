using MD.Diggable.Projectile;
using System;
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
        EventSystems.EventManager.Instance.StartListening<ProjectilePickupData>(SwapProjHoldSprite);
    }

    
    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectilePickupData>(SwapProjHoldSprite);
    }

    private void SwapProjHoldSprite(ProjectilePickupData obj)
    {
        spriteRenderer.sprite = projectileHoldSprite;
    }

}
