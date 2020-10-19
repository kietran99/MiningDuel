using MD.Diggable.Projectile;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ThrowControl : MonoBehaviour
{
    [SerializeField]
    private Button button = null;

    private void Start()
    {
        button.onClick.AddListener(Invoke);
        EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(ShowButton);
    }
    
    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(ShowButton);
    }

    private void Invoke()
    {
        EventSystems.EventManager.Instance.TriggerEvent(new ThrowInvokeData());
    }

    private void ShowButton(ProjectileObtainData obj)
    {
        button.gameObject.SetActive(true);
    }
}
