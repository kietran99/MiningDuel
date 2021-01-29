using MD.Diggable.Core;
using MD.Diggable.Gem;
using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class GemObtainEffect : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private void Start() 
        {
            spriteRenderer = GetComponent<SpriteRenderer>();  
            animator = GetComponent<Animator>(); 

            EventSystems.EventManager.Instance.StartListening<GemDugData>(HandleGemDug);
        }

        private void OnDisable() 
        {
            EventSystems.EventManager.Instance.StopListening<GemDugData>(HandleGemDug);
        }

        private void HandleGemDug(GemDugData gemDugData)
        {
            var gem = DiggableTypeConverter.Convert((DiggableType) gemDugData.value);
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
