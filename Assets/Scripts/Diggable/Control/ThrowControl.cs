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
        EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(HideButton);
        EventSystems.EventManager.Instance.StartListening<ExplodeData>(HideButton);
    }

    private void Invoke()
    { 
        EventSystems.EventManager.Instance.TriggerEvent(new ThrowInvokeData());
    }

    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(ShowButton);
        EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(HideButton);
        EventSystems.EventManager.Instance.StopListening<ExplodeData>(HideButton);
    }

    private void HideButton(ExplodeData obj)
    {
        button.gameObject.SetActive(false);
    }

    private void HideButton(ThrowInvokeData obj)
    {
        button.gameObject.SetActive(false);
    }
    
    private void ShowButton(ProjectileObtainData obj)
    {
        button.gameObject.SetActive(true);
    }
}
