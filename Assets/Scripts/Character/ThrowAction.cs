using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;

public class ThrowAction : MonoBehaviour
{      
    [SerializeField]
    private float baseDistance = 1f;

    private Vector2 throwDir;
    private GameObject projectile;

    void Start()
    {
        EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(ThrowProjectile);
        EventSystems.EventManager.Instance.StartListening<JoystickDragData>(BindThrowDirection);
    }

    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ThrowProjectile);
        EventSystems.EventManager.Instance.StopListening<JoystickDragData>(BindThrowDirection);
    }

    private void BindThrowDirection(JoystickDragData dragData)
    {
        if (projectile == null) return;

        projectile.GetComponent<ProjectileLauncher>().BindThrowDirection(
            new Vector2(dragData.InputDirection.x, dragData.InputDirection.y));
    }

    public void BindProjectile(GameObject projectile) => this.projectile = projectile;

    private void ThrowProjectile(ThrowInvokeData data)
    {
        projectile.GetComponent<ProjectileLauncher>().Throw(baseDistance);
        projectile = null;
    }
}
