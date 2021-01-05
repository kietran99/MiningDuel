using UnityEngine;

public class TestInvoke : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventSystems.EventManager.Instance.TriggerEvent<TestVoidEvent>();
        }

        else if (Input.GetKeyDown(KeyCode.A))
        {
            EventSystems.EventManager.Instance.TriggerEvent<TestArgEvent, int>(1);
        }
    }
}
