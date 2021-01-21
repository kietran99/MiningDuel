using UnityEngine;

public class TestInvoke : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventSystems.EventManager.Instance.TriggerEvent<TestArgEvent>(new TestArgEvent());
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            EventSystems.EventManager.Instance.TriggerEvent<NewTestEvent>(new NewTestEvent(2));
        }
    }
}
