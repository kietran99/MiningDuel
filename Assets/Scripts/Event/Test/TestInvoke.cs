using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TestInvoke : MonoBehaviour
{
    // void Start()
    // {
    //     // var sw = new System.Diagnostics.Stopwatch();
    //     // long sum = 0;
    //     // // var nEvents = 10000000;
    //     // var nEvents = 1;
    //     // var nRuns = 1;

    //     // for (int j = 0; j < nRuns; j++)
    //     // {
    //     //     sw.Start();
    //     //     for (int i = 0; i < nEvents; i++) 
    //     //     {
    //     //         EventSystems.EventManager.Instance.TriggerEvent(new NewTestEvent(10, 5.845f));
    //     //     }
    //     //     sw.Stop();
    //     //     Debug.Log(sw.ElapsedMilliseconds);
    //     //     sum += sw.ElapsedMilliseconds;
    //     //     sw.Reset();
    //     // }

    //     // Debug.Log("AVG: " + sum / nRuns);
    // }

    // private readonly string INVOKE = "INVOKE";

    async void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     EventSystems.EventManager.Instance.TriggerEvent(new NewTestEvent(10, 5.845f));
        // }
        // EventSystems.EventManager.Instance.TriggerEvent(new NewTestEvent(10, 5.845f));
        await EventSystems.EventManager.Instance.TriggerEventAsync(new NewTestEvent(10, 5.845f));
        // Debug.Log(INVOKE);
    }
}
