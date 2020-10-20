using UnityEngine;

public class UsernameContainer : MonoBehaviour
{
    private void Start()
    {
        EventSystems.EventManager.Instance.StartListening<RoomWindowToggleData>(Toggle);
    }

    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<RoomWindowToggleData>(Toggle);
    }

    private void Toggle(RoomWindowToggleData obj)
    {
        gameObject.SetActive(!obj.isActive);
    }
}
