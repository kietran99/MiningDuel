using UnityEngine;
using EventSystems;
using System;

public class TestListen : MonoBehaviour
{
    void Start()
    {
        var eventConsumer = GetComponent<EventConsumer>();
        eventConsumer.StartListening<TestArgEvent>(HandleEvent);
        eventConsumer.StartListening<NewTestEvent>(HandleNewTestEvent);
    }

    private void HandleNewTestEvent(NewTestEvent obj)
    {
        Debug.Log(name + ": " + obj.x);
    }

    void HandleEvent(TestArgEvent data)
    {
        Debug.Log(name);
    }
}
