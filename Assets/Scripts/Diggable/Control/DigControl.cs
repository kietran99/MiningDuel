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
            EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(HideButton);
        }

        
        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(HideButton);
        }

        public void Invoke()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData());
        }

        private void HideButton(ProjectileObtainData obj)
        {
            button.gameObject.SetActive(false);
        }
    }
}
