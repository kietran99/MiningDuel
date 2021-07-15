using UnityEngine;

public class TestInvoke : MonoBehaviour
{
    void Start()
    {
        var sw = new System.Diagnostics.Stopwatch();
        long sum = 0;
        var nEvents = 10000000;
        var nRuns = 10;

        for (int j = 0; j < nRuns; j++)
        {
            sw.Start();
            for (int i = 0; i < nEvents; i++) 
            {
                EventSystems.EventManager.Instance.TriggerEvent<NewTestEvent>(new NewTestEvent(10, 5.845f));
            }
            sw.Stop();
            Debug.Log(sw.ElapsedMilliseconds);
            sum += sw.ElapsedMilliseconds;
            sw.Reset();
        }

        Debug.Log("AVG: " + sum / nRuns);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventSystems.EventManager.Instance.TriggerEvent<NewTestEvent>(new NewTestEvent(10, 5.845f));
        }
    }
}
