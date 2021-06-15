using UnityEngine;
using UnityEngine.Events;

namespace Utils.UI
{
    public class UISpriteAnimationControl : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] frames = null;

        [SerializeField]
        private UnityEngine.UI.Image image = null;

        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private int frameRate = 30;

        [SerializeField]
        private bool playOnStart = false;

        [SerializeField]
        private bool disableOnEnd = true;

        [SerializeField]
        private bool interruptable = false;

        public UnityEvent OnEnd;

        private bool shouldPlay;
        private float timePerFrame;
        private float elapsed;
        private int curFrameIdx;

        public void Play()
        {
            if (shouldPlay && !interruptable)
            {
                return;
            }

            shouldPlay = true;
            image.sprite = frames[0];
            Init();
        }

        void Start()
        {
            timePerFrame = 1f / frameRate;
            Init();
            shouldPlay = playOnStart;

            if (!playOnStart)
            {
                DisableImg();
            }
        }

        void Update()
        {
            if (!shouldPlay)
            {
                return;
            }

            elapsed += Time.deltaTime * speed;

            if (elapsed < timePerFrame)
            {
                return;
            }

            elapsed = 0f;

            if (++curFrameIdx >= frames.Length)
            {
                if (disableOnEnd)
                {
                    DisableImg();
                }

                shouldPlay = false;
                OnEnd?.Invoke();
                return;
            }

            image.sprite = frames[curFrameIdx];
        }

        private void Init()
        {
            elapsed = 0f;
            curFrameIdx = 0;   
            image.enabled = true;
        }

        private void DisableImg()
        {
            image.sprite = null;
            image.enabled = false;
        }     
    }
}
