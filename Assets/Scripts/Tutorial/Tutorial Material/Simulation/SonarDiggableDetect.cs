using UnityEngine;

namespace MD.Tutorial
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class SonarDiggableDetect : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<SpriteMask>() == null)
            {
                return;
            }

            EventSystems.EventManager.Instance.TriggerEvent(new DiggableVisibleData());
        }
    }
}
