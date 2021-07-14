using UnityEngine;
using UnityEngine.Events;

namespace Utils.UI
{
    public class SwipeIndicator : MonoBehaviour
    {
        [SerializeField]
        private float fadeSpeed = 1f;

        [SerializeField]
        private UnityEngine.UI.Image image = null;

        [SerializeField]
        private UnityEvent OnFadeOut = null;

        private bool swipeLeft = false;
        private bool shouldFade = false;

        public void FadeOut() // Ref in editor
        {      
            shouldFade = true;
        }

        private void Update()
        {
            if (!shouldFade)
            {
                return;
            }

            image.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(image.color.a, 0f, Time.deltaTime * fadeSpeed));

            if (image.color.a == 0f)
            {
                shouldFade = false;
                image.color = new Color(1f, 1f, 1f, 1f);
                transform.eulerAngles = new Vector3(0f, swipeLeft ? 0f : 180f, 0f);
                swipeLeft = !swipeLeft;
                OnFadeOut?.Invoke();
            }
        }
    }
}
