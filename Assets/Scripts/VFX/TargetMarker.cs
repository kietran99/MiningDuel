using UnityEngine;

namespace MD.UI
{
    public class TargetMarker : MonoBehaviour
    {
        private Transform target;
        private bool isDigging;
        private Vector2 attackTargetPos;

        private void Start()
        {
            isDigging = true;
            target = ServiceLocator.Resolve<Character.Player>().Match(err => transform, player => player.transform);
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.AttackTargetChangeData>(ToggleMode);
        }

        private void FixedUpdate()
        {
            if (isDigging)
            {
                transform.position = new Vector3(Mathf.FloorToInt(target.position.x) + .5f, Mathf.FloorToInt(target.position.y) + .5f, 0f);
                return;
            }

            transform.position = attackTargetPos;
        }

        private void ToggleMode(Character.AttackTargetChangeData data)
        {
            if (!data.attackable)
            {
                isDigging = true;
                return;
            }

            isDigging = false;
            attackTargetPos = data.targetPos;
        }
    }
}
