using System.Collections;
using UnityEngine;

namespace Timer
{
    public class Timer : MonoBehaviour, ITimer
    {
        private enum TimerState { ON, OFF }

        [SerializeField]
        private float[] timeStamps = null;

        private TimerState state = TimerState.OFF;
        private ITickListener listener;

        void Start()
        {
            listener = GetComponent<ITickListener>();
        }
       
        private IEnumerator Tick()
        {

            while (state.Equals(TimerState.ON))
            {     
                for (int i = 0; i < timeStamps.Length; i++)
                {
                    yield return new WaitForSecondsRealtime(timeStamps[i]);

                    listener.OnTick(timeStamps[i]);
                }               
            }
        }

        public void Activate()
        {       
            state = TimerState.ON;
            StartCoroutine(Tick());
        }

        public void Stop()
        {            
            StopCoroutine(Tick());
            state = TimerState.OFF;
        }
    }
}