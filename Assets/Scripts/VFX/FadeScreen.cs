using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(Animator))]
    public class FadeScreen : MonoBehaviour
    {        
        private Animator theAnimator;
        private event System.Action fadeStartCompleteHandler;

        void Start()
        {
            ServiceLocator.Register<FadeScreen>(this);
            theAnimator = GetComponent<Animator>();
            DontDestroyOnLoad(gameObject);
        }

        public void StartFading(System.Action fadeStartCompleteHandler = null)
        {     
            this.fadeStartCompleteHandler = fadeStartCompleteHandler;           
            theAnimator.SetTrigger("Start");
        }       

        public void HandleFadeEndComplete()
        {
            
        }

        public void HandleFadeStartComplete()
        {    
            fadeStartCompleteHandler?.Invoke();    
            theAnimator.SetTrigger("End");
        }
    }
}
