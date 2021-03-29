using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class IsMouseNearby : BTLeaf
    {
        [SerializeField]
        private float detectionRange = 7f;

        private Camera mainCamera;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            if (mainCamera == null) mainCamera = Camera.main;

            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            blackboard.Set<Vector2>("MousePos", mousePos);
            var dist = Mathf.Pow(actor.transform.position.x - mousePos.x, 2) + Mathf.Pow(actor.transform.position.y - mousePos.y, 2);

            return dist <= detectionRange * detectionRange ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}
