using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utils.UI
{
    public class SwipeHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private class Logger
        {
            private bool isActive;

            public Logger(bool isActive)
            {
                this.isActive = isActive;
            }

            public void Log(string msg)
            {
                if (!isActive)
                {
                    return;
                }

                Debug.Log(msg);
            }
        }

        [SerializeField]
        private bool logActive = false;

        [SerializeField]
        private float detectDistance = 2f;

        [SerializeField]
        private UnityEvent onSwipeLeft = null;

        [SerializeField]
        private UnityEvent onSwipeRight = null;

        private Logger logger;
        private float downX = 0f;

        private readonly string SWIPE_DELTA = "Swipe Delta ";
        private readonly string ON_SWIPE_LEFT = "On Swipe Left";
        private readonly string ON_SWIPE_RIGHT = "On Swipe Right";

        void Start()
        {
            logger = new Logger(logActive);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            downX = eventData.position.x;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var upX = eventData.position.x;
            logger.Log(SWIPE_DELTA + (upX - downX));

            if (Mathf.Abs(upX - downX) <= detectDistance)
            {
                return;
            }

            if (upX < downX)
            {
                logger.Log(ON_SWIPE_LEFT);
                onSwipeLeft?.Invoke();
            }
            else
            {   
                logger.Log(ON_SWIPE_RIGHT);
                onSwipeRight?.Invoke();
            }
        }        
    }
}
