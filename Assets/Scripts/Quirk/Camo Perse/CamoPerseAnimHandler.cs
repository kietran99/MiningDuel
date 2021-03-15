using UnityEngine;

namespace MD.Quirk
{
    public class CamoPerseAnimHandler : MonoBehaviour
    {
        [SerializeField]
        private Animator theAnimator = null;

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private Sprite[] sonarSprites = null;

        public void PlayAnimation()
        {
            theAnimator.enabled = true;
        }

        public void TriggerAnimEndEvent()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new CamoPerseAnimEndData());
            theAnimator.enabled = false;
            spriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            spriteRenderer.sprite = sonarSprites.Random();
        }
    }
}
