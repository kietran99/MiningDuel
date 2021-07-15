using UnityEngine;

namespace MD.UI
{
    public class UseItemControl: MonoBehaviour
    {
        [SerializeField]
        private GameObject Button = null;

        void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<SetCraftButtonData>(HandleSetButtonData);
        }

        public void HandleSetButtonData(SetCraftButtonData data)
        {
            if (data.status)
            {
                if (!Button.activeInHierarchy)
                {
                    Button.SetActive(true);
                }
            }
            else
            {
                if (Button.activeInHierarchy)
                {
                    Button.SetActive(false);
                }
            }
        }

        public void UseItem()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new UseItemInvokeData());
        }
    }
}
