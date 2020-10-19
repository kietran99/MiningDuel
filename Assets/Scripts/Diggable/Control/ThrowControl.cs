using MD.Diggable.Projectile;
using UnityEngine;
using UnityEngine.UI;

public class ThrowControl : MonoBehaviour
{
    [SerializeField]
    private Button button = null;

    private void Start()
    {
        EventSystems.EventManager.Instance.StartListening<ProjectilePickupData>(ShowButton);
    }
    
    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectilePickupData>(ShowButton);
    }

    private void ShowButton(ProjectilePickupData obj)
    {
        button.gameObject.SetActive(true);
    }
}
