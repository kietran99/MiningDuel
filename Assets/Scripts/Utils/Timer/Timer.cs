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
        private bool startTicking = false;
        private int curStampIdx = 0;
        private float curCounter = 0f;

        void Start()
        {
            listener = GetComponent<ITickListener>();
        }

        void Update()
        {
            // if (!startTicking) return;

            // if (timeStamps.Length == 0) return;

            // if (curStampIdx == timeStamps.Length)
            // {
            //     startTicking = false;
            //     return;
            // }

            // curCounter += Time.deltaTime;

            // if (curCounter < timeStamps[curStampIdx]) return;
            
            // listener.OnTick(timeStamps[curStampIdx]);
            // curStampIdx++;                      
        }

        private IEnumerator Tick()
        {
            Debug.Log("Start coroutine");    

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
            Debug.Log("Activate timer");        
            state = TimerState.ON;
            StartCoroutine(Tick());
            //startTicking = true;
        }

        public void Stop()
        {            
            StopCoroutine(Tick());
            //startTicking = false;
            state = TimerState.OFF;
        }
    }
}