using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialPlayer : MonoBehaviour
    {
        private void Start()
        {
            gameObject.AddComponent<EventSystems.EventConsumer>();
        }
    }
}