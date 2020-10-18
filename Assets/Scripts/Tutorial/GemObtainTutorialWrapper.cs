using UnityEngine;

namespace MD.Tutorial
{
    public class GemObtainTutorialWrapper : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            EventSystems.EventManager.Instance.TriggerEvent(new GemCollideTutorialData());
        }        
    }
}