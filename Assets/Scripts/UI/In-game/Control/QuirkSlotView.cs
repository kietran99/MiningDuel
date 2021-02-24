using UnityEngine;
using UnityEngine.UI;
using MD.Quirk;

namespace MD.UI
{
    public class QuirkSlotView : MonoBehaviour
    {
        [SerializeField]
        private int quirkSlotIndex = 0;

        [SerializeField]
        private GameObject activeBorder = null, inactiveBorder = null;
        
        [SerializeField]
        private Button quirkButton = null;
        
        [SerializeField]
        private Image quirkImage = null; 

        private void Start()
        {
            EventSystems.EventManager.Instance.StartListening<QuirkObtainData>(HandleQuirkObtainEvent);
            quirkButton.onClick.AddListener(Activate);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<QuirkObtainData>(HandleQuirkObtainEvent);
            quirkButton.onClick.RemoveListener(Activate);
        }

        private void Activate()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new QuirkInvokeData(quirkSlotIndex));
            Remove();
        }

        private void HandleQuirkObtainEvent(QuirkObtainData quirkObtainData)
        {
            Insert(quirkObtainData.quirkSprite);
        }

        private void Insert(Sprite quirkSprite)
        {
            inactiveBorder.SetActive(false);
            activeBorder.SetActive(true);
            quirkButton.gameObject.SetActive(true);
            quirkImage.sprite = quirkSprite;
        }

        private void Remove()
        {
            inactiveBorder.SetActive(true);
            activeBorder.SetActive(false);
            quirkButton.gameObject.SetActive(false);
            quirkImage.sprite = null;
        }
    }
}
