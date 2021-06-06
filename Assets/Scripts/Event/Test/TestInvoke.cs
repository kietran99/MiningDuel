using UnityEngine;

public class TestInvoke : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventSystems.EventManager.Instance.TriggerEvent<NewTestEvent>(new NewTestEvent(10, 5.845f));
        }
    }
}
