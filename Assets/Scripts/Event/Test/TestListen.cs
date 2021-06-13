using UnityEngine;
using EventSystems;
using System.Threading;

public class TestListen : MonoBehaviour
{
    public EventConsumer consumer;

    int x = 0;
    void Start()
    {
        consumer.StartListening<NewTestEvent>(TestCallback);
        
        // consumer.StartListening<NewTestEvent>(_ => {});
        // consumer.StartListening<NewTestEvent>(OnEvent);
        // consumer.StartListeningWithMap<NewTestEvent, int>(data => data.myInt, OnEvent);
        // consumer.StartListening<NewTestEvent, float>(OnEvent, data => data.myFloat);
        // consumer.StartListening<NewTestEvent>(OnEvent, data => data.myInt < 0);
        // consumer.StartListening<NewTestEvent, string>(data => Debug.Log("String: " + data), data => data.myInt > 10 ? "> 10" : "<= 10", data => data.myInt > 0);
    }

    private void OnEvent(NewTestEvent data) => Debug.Log("On Event");

    private void OnEvent(int data) => Debug.Log("Int: " + data);

    private void OnEvent(float data) => Debug.Log("Float: " + data);

    private void TestCallback(NewTestEvent _)
    // {
    //     Thread.Sleep(3000); 
    //     } 
    // {for (int i = 0; i < 10000000; i++) x = 1;}
    {}
}
