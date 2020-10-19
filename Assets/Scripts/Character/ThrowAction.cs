using MD.UI;
using System;
using UnityEngine;

public class ThrowAction : MonoBehaviour
{      
    [SerializeField]
    private float baseDistance = 1f;

    private GameObject projectile;
    private Vector2 throwDir;

    void Start()
    {
        EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(ThrowProjectile);
        EventSystems.EventManager.Instance.StartListening<JoystickDragData>(BindThrowDir);
    }

    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ThrowProjectile);
        EventSystems.EventManager.Instance.StopListening<JoystickDragData>(BindThrowDir);
    }

    private void BindThrowDir(JoystickDragData dragData)
    {
        throwDir = new Vector2(dragData.InputDirection.x, dragData.InputDirection.y);
    }

    public void BindProjectile(GameObject projectile) => this.projectile = projectile;

    private void ThrowProjectile(ThrowInvokeData data)
    {
        //projectile.GetComponent<Rigidbody2D>().AddForce(throwDir.normalized * baseDistance, ForceMode2D.Impulse);
    }
}
