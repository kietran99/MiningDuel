using MD.Diggable.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent (typeof(Button))]
    public class DigControl : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Invoke);
        }

        private void Start()
        {
            EventSystems.EventManager.Instance.StartListening<ProjectilePickupData>(HideButton);
        }

        
        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ProjectilePickupData>(HideButton);
        }

        public void Invoke()
        {
            EventSystems.EventManager.Instance.TriggerEvent<DigControlData>(new DigControlData());
        }

        private void HideButton(ProjectilePickupData obj)
        {
            button.gameObject.SetActive(false);
        }
    }
}
