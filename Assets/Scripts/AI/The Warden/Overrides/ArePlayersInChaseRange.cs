using MD.AI.BehaviourTree;
using UnityEngine;
using Functional.Type;

namespace MD.AI.TheWarden
{
    public class ArePlayersInChaseRange : BTLeaf
    {
        private class ChaseTarget
        {
            private Transform transform;
            private Character.IScoreManager scoreManager;

            public ChaseTarget(Transform transform, Character.IScoreManager scoreManager)
            {
                this.transform = transform;
                this.scoreManager = scoreManager;
            }

            public static Option<ChaseTarget> New(Transform transform)
            {
                var maybeScoreManager = transform.GetComponent<Character.IScoreManager>();
                if (maybeScoreManager == null)
                {
                    Debug.LogError("No IScoreManager derived script attached to this GameObject");
                    return Option<ChaseTarget>.None;
                }

                return new ChaseTarget(transform, maybeScoreManager);
            }

            public Transform Transform => transform;

            public Vector3 Position => transform.position;

            public int Score => scoreManager.CurrentScore;
        }

        [SerializeField]
        private float baseChaseRange = 7f;

        [SerializeField]
        private Transform[] players = null; // For testing targets

        private bool hasSetup = false;
        
        private ChaseTarget[] targets;

        private float chaseRange;

        private void Start()
        {
            var maybeValidTargets = players.Map(player => ChaseTarget.New(player));
            if (maybeValidTargets.Length != players.Length)
            {
                return;
            }

            var targets = new System.Collections.Generic.List<ChaseTarget>();
            maybeValidTargets.ForEach(maybeTarget => maybeTarget.Match(target => targets.Add(target), () => {}));
            this.targets = targets.ToArray();
            chaseRange = baseChaseRange;
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            if (!hasSetup)
            {
                blackboard.Set<float>(WardenMacros.CHASE_RANGE, baseChaseRange);
                hasSetup = true;
            }

            return blackboard
                .Get<float>(WardenMacros.CHASE_RANGE)
                .Match(
                    chaseRange => 
                    {
                        var chasables = targets.Filter(target => (actor.transform.position - target.Position).sqrMagnitude <= chaseRange * chaseRange);

                        if (chasables.Length == 0)
                        {
                            return BTNodeState.FAILURE;
                        }

                        var chaseTarget = chasables.Reduce((target_0 , target_1) => target_0.Score > target_1.Score ? target_0 : target_1);
                        blackboard.Set<Transform>(WardenMacros.CHASE_TARGET, chaseTarget.Transform);              
                        BTLogger.Log("Number of Chasable Players: " + chasables.Length);
                        this.chaseRange = chaseRange;
                        return BTNodeState.SUCCESS;
                    },
                    () => BTNodeState.FAILURE
                );
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}
