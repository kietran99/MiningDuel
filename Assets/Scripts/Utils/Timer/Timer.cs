using System.Collections;
using UnityEngine;

namespace Timer
{
    public class Timer : MonoBehaviour, ITimer
    {
        private enum TimerState { ON, OFF }

        [SerializeField]
        private float interval = 2f;

        [SerializeField]
        private TimerState state = TimerState.ON;

        private ITickListener listener;

        void Start()
        {
            listener = GetComponent<ITickListener>();
        }

        private IEnumerator Tick()
        {
            var secToNextTick = new WaitForSecondsRealtime(interval);

            while (state.Equals(TimerState.ON))
            {
                listener.OnTick();

                yield return secToNextTick;
            }
        }

        public void Activate()
        {
            if (listener == null)
            {
                Debug.LogError("Can't find ITickListener script on this GameObject");
                return;
            }

            state = TimerState.ON;
            StartCoroutine(Tick());
        }

        public void Stop()
        {
            if (listener == null)
            {
                Debug.LogError("Can't find ITickListener script on this GameObject");
                return;
            }

            StopCoroutine(Tick());
            state = TimerState.OFF;
        }
    }
}