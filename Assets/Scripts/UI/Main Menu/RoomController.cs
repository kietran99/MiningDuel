using UnityEngine;

namespace MD.UI.MainMenu
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField]
        private GameObject container = null;

        public void ShowWindow()
        {
            container.SetActive(true);
            EventSystems.EventManager.Instance.TriggerEvent(new RoomWindowToggleData(true));
        }

        public void Exit()
        {
            container.SetActive(false);
            EventSystems.EventManager.Instance.TriggerEvent(new RoomWindowToggleData(false));
        }
    }
}