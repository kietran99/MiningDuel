﻿using MD.Diggable.Projectile;
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
        EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(SwapProjHoldSprite);
        EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(SwapIdleSprite);
        EventSystems.EventManager.Instance.StartListening<ExplodeData>(SwapIdleSprite);
    }

    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(SwapProjHoldSprite);
        EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(SwapIdleSprite);
        EventSystems.EventManager.Instance.StopListening<ExplodeData>(SwapIdleSprite);
    }

    private void SwapProjHoldSprite(ProjectileObtainData obj)
    {
        spriteRenderer.sprite = projectileHoldSprite;
    }

    private void SwapIdleSprite(ThrowInvokeData obj)
    {
        spriteRenderer.sprite = idleSprite;
    }

    private void SwapIdleSprite(ExplodeData obj)
    {
        spriteRenderer.sprite = idleSprite;
    }
}
