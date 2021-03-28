using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class GoToMousePosition : BTLeaf
    {
        private Camera mainCamera;

        protected override BTNodeState DecoratedTick(GameObject actor)
        {
            if (mainCamera == null) mainCamera = Camera.main;

            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);        
            Vector2 moveDir = (mousePos - new Vector2(actor.transform.position.x, actor.transform.position.y)).normalized * 5 * Time.deltaTime;
            actor.transform.Translate(moveDir);
            return BTNodeState.SUCCESS;
        }
    }
}
