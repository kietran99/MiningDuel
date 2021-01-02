using MD.Diggable;
using MD.Diggable.Core;
using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class GemObtainEffect : MonoBehaviour
    {
        [SerializeField]
        private MD.Character.Player player = null;

        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private void Start() 
        {
            spriteRenderer = GetComponent<SpriteRenderer>();  
            animator = GetComponent<Animator>(); 

            if (!player.isLocalPlayer) return; 

            EventSystems.EventManager.Instance.StartListening<GemDigSuccessData>(HandleGemDigSuccess);
        }

        private void OnDisable() 
        {
            if (!player.isLocalPlayer) return;

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
