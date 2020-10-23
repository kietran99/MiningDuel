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
            if (!startTicking) return;

            if (timeStamps.Length == 0) return;

            if (curStampIdx == timeStamps.Length)
            {
                startTicking = false;
                return;
            }

            curCounter += Time.deltaTime;

            if (curCounter >= timeStamps[curStampIdx])
            {
                listener.OnTick(timeStamps[curStampIdx]);
                curStampIdx++;
                return;
            }           
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
            //StartCoroutine(Tick());
            startTicking = true;
        }

        public void Stop()
        {            
            //StopCoroutine(Tick());
            startTicking = false;
            state = TimerState.OFF;
        }
    }
}