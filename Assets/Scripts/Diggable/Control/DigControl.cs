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
            EventSystems.EventManager.Instance.StartListening<ExplodeData>(ShowButton);
        }

        public void Invoke()
        {
            Debug.Log("dig button clicked");
            EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData());
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(HideButton);
            EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ShowButton);
            EventSystems.EventManager.Instance.StopListening<ExplodeData>(ShowButton);
        }

        private void ShowButton(ExplodeData obj)
        {
            button.gameObject.SetActive(true);
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
