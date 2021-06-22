using UnityEngine;
using EventSystems;
using System;

public class TestListen : MonoBehaviour
{
    public EventConsumer consumer;

    int x = 0;

    void Start()
    {
        // consumer.StartListening<NewTestEvent>(TestCallback);
        
        EventHub.Instance.Get<TestTupleEvent>().Subscribe(OnEvent);
    }

    private void OnEvent((int id, int score, bool isMale) data)
    {
        // Debug.Log(data);
    }

    private void TestCallback(NewTestEvent data)
    {
        // Debug.Log(data);
        // var cnt = 0;
        // for (int i = 0; i < 1000000; i++)
        // {
        //     cnt++;
        // }

        // Debug.Log(++x);
    }
    // {}
}
