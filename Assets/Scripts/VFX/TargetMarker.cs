using UnityEngine;

namespace MD.UI
{
    public class TargetMarker : MonoBehaviour
    {
        private Transform target;

        private void Start()
        {
            target = ServiceLocator.Resolve<Character.Player>().Match(err => transform, player => player.transform);
        }

        private void FixedUpdate()
        {
            transform.position = new Vector3(Mathf.FloorToInt(target.position.x) + .5f, Mathf.FloorToInt(target.position.y) + .5f, 0f);
        }
    }
}
