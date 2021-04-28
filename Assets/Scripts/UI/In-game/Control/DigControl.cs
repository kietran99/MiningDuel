using MD.Diggable.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class DigControl : MonoBehaviour
    {
        [SerializeField]
        private Button button = null;
        
        private void Start()
        {
            button.onClick.AddListener(Invoke);
            EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(HideButton);
            EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(ShowButton);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Invoke);
            EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(HideButton);
            EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ShowButton);
        }

        public void Invoke()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData());
        }

        private void ShowButton(ThrowInvokeData obj)
        {
            button.gameObject.SetActive(true);
        }

        private void HideButton(ProjectileObtainData obj)
        {
            button.gameObject.SetActive(false);
        }
    }
}
