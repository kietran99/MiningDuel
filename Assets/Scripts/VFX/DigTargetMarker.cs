using UnityEngine;

namespace MD.VisualEffects
{
    public class DigTargetMarker : MonoBehaviour
    {
        private Transform target;

        private void Start()
        {
            target = ServiceLocator.Resolve<Character.Player>().Match(err => transform, player => player.transform);
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(Mathf.FloorToInt(target.position.x) + .5f, Mathf.FloorToInt(target.position.y) + .5f, 0f);
        }
    }
}
