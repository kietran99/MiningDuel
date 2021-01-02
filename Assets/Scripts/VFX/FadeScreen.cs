using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(Animator))]
    public class FadeScreen : MonoBehaviour
    {
        private Animator theAnimator;
        private Animator TheAnimator
        {
            get
            {
                if (!theAnimator.enabled) theAnimator.enabled = true;
                return theAnimator;
            }
        }

        void Start()
        {
            theAnimator = GetComponent<Animator>();
            DontDestroyOnLoad(gameObject);
        }

        public void StartFading()
        {
            theAnimator.enabled = true;
        }

        public void SetAnimatorTrigger(string name)
        {
            TheAnimator.SetTrigger(name);
        }

        public void HandleFadeEndComplete()
        {
            Destroy(gameObject);
        }

        public void HandleFadeStartComplete()
        {                      
            EventSystems.EventManager.Instance.TriggerEvent(new FadeStartCompleteData());
            theAnimator.SetTrigger("Start");
        }
    }
}
