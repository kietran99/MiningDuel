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
            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StartListening<ProjectileObtainData>(HideButton);
            eventManager.StartListening<ThrowInvokeData>(ShowButton);
            eventManager.StartListening<ExplodeData>(ShowButton);
            //eventManager.StartListening<DigSpeedData>(SetButtonCooldown);
        }

        public void Invoke()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData());
        }

        private void OnDestroy()
        {
            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StopListening<ProjectileObtainData>(HideButton);
            eventManager.StopListening<ThrowInvokeData>(ShowButton);
            eventManager.StopListening<ExplodeData>(ShowButton);
            //eventManager.StopListening<DigSpeedData>(SetButtonCooldown);
        }

        //private void SetButtonCooldown(DigSpeedData digSpeed) => button.GetComponent<PixelatedButton>().Cooldown = digSpeed.speed;

        private void ShowButton(ExplodeData obj) => button.gameObject.SetActive(true);

        private void ShowButton(ThrowInvokeData obj) => button.gameObject.SetActive(true);

        private void HideButton(ProjectileObtainData obj) => button.gameObject.SetActive(false);
    }
}
