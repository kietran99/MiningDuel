using MD.Diggable;
using MD.Diggable.Core;
using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class GemObtainEffect : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.T)) Play(null);
        }

        private void Start() 
        {
            spriteRenderer = GetComponent<SpriteRenderer>();  
            animator = GetComponent<Animator>();  
            EventSystems.EventManager.Instance.StartListening<GemDigSuccessData>(HandleGemDigSuccess);
        }

        private void OnDestroy() 
        {
            EventSystems.EventManager.Instance.StopListening<GemDigSuccessData>(HandleGemDigSuccess);
        }

        private void HandleGemDigSuccess(GemDigSuccessData gemDigData)
        {
            var gem = DiggableTypeConverter.Convert((DiggableType) gemDigData.value);
            Play(gem.WorldSprite);
        } 

        private void Play(Sprite gemSprite)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = gemSprite;
            animator.enabled = true;
        }

        public void Stop() => animator.enabled = false;        
    }
}
