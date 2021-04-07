using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenRoot : BTRoot
    {
        [SerializeField]
        private Transform[] players = null; // FOR TESTING ONLY

        [SerializeField]
        private UnityEngine.Tilemaps.Tilemap map = null; // Used as a temporary to assign values to topRightLimit & botLeftLimit

        protected override void SetupAdditionalStates(BTBlackboard blackboard)
        {
            blackboard.Set<Transform[]>(WardenMacros.PLAYERS, players);
            blackboard.Set<float>(WardenMacros.ATTACK_COOLDOWN, 0f);
            blackboard.Set<(Vector2, Vector2)>(WardenMacros.MAP_LIMITS, (map.localBounds.max - new Vector3(.5f, .5f, 0f), map.localBounds.min + new Vector3(.5f, .5f, 0f)));
            blackboard.Set<ParticleSystem>(WardenMacros.CHASE_PARTICLES, actor.GetComponentInChildren<ParticleSystem>());
            blackboard.Set<Animator>(WardenMacros.ANIMATOR, actor.GetComponentInChildren<Animator>());
            blackboard.Set<float>(WardenMacros.DELTA_CHASE_RANGE, 0);   
        }
    }
}
