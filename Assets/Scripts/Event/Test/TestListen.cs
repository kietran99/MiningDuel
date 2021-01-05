using UnityEngine;
using EventSystems;

public class TestListen : MonoBehaviour
{
    void Start()
    {
        EventManager.Instance.StartListening<TestVoidEvent>(HandleVoidEvent);
        EventManager.Instance.StartListening<TestArgEvent, int>(HandleArgEvent);
    }

    void OnDestroy()
    {
        EventManager.Instance.StopListening<TestVoidEvent>(HandleVoidEvent);
        EventManager.Instance.StopListening<TestArgEvent, int>(HandleArgEvent);
    }

    void HandleVoidEvent()
    {
        Debug.Log(gameObject.name);
    }

    void HandleArgEvent(int val)
    {
        Debug.Log(gameObject.name + ": " + val);
    }
}
