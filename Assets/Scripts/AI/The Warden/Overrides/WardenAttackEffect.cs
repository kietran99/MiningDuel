using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenAttackEffect : BTLeaf
    {
        private SpriteRenderer spriteRenderer;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            Play(actor);
            return BTNodeState.SUCCESS;
        }

        private void Play(GameObject actor)
        {
            if (spriteRenderer == null) spriteRenderer = actor.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.enabled = true;
            Invoke(nameof(HideSprite), .5f);
        }

        private void HideSprite() => spriteRenderer.enabled = false;
    }
}
