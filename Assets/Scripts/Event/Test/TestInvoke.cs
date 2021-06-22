// using System.Diagnostics;
// using System.Threading;
// using System.Threading.Tasks;
// using System;
using UnityEngine;
using EventSystems;
using System;

public class TestInvoke : MonoBehaviour
{
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     EventSystems.EventManager.Instance.TriggerEvent(new NewTestEvent(10, 5.845f));
        // }

        // EventSystems.EventManager.Instance.TriggerEvent(new NewTestEvent(10, 5.845f));
        EventHub.Instance.Get<TestTupleEvent>().Publish((1001, 24, true));
    }
}

public class TestTupleEvent : TupleEvent<(int id, int score, bool isMale)> {}
