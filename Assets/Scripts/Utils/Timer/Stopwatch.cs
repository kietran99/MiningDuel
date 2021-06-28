using UnityEngine;

namespace Utils.Misc
{
    public class Stopwatch : MonoBehaviour
    {
        private float _elapsed;
        private bool _interruptable;
        
        public UnityEngine.Events.UnityAction OnStop { get; set; }

        public bool IsActive => _elapsed > 0f && enabled;

        private void Start()
        {
            _elapsed = 0f;
            _interruptable = false;
            enabled = false;
        }

        public void Begin(float seconds, bool interruptable = false)
        {
            if (_elapsed > 0f && !interruptable)
            {
                return;
            }

            _interruptable = interruptable;
            _elapsed = seconds;
            enabled = true;
        }

        void Update()
        {
            if (_elapsed > 0f)
            {
                _elapsed -= Time.deltaTime;
                return;
            }

            OnStop?.Invoke();
            enabled = false;
        }
    }
}
