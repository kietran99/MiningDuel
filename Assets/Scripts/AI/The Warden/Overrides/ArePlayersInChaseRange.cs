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
        
        private ChaseTarget[] targets;

        private float gizmosChaseRange;

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
            gizmosChaseRange = baseChaseRange;
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var chaseRange = blackboard
                .Get<float>(WardenMacros.DELTA_CHASE_RANGE)
                .Match(
                    deltaChaseRange => baseChaseRange + deltaChaseRange,
                    () => baseChaseRange
                ); 

            gizmosChaseRange = chaseRange;

            var maybeTarget = CheckTargetsInRange(actor.transform.position, chaseRange);   

            return 
                maybeTarget
                    .Match(
                        chaseTarget =>
                        {
                            blackboard.Set<Transform>(WardenMacros.CHASE_TARGET, chaseTarget);
                            return BTNodeState.SUCCESS;
                        },
                        () => BTNodeState.FAILURE
                    );              
        }

        private Option<Transform> CheckTargetsInRange(Vector3 actorPos, float chaseRange)
        {
            var chasables = targets.Filter(target => (actorPos - target.Position).sqrMagnitude <= chaseRange * chaseRange);

            if (chasables.Length == 0)
            {
                return Option<Transform>.None;
            }

            var chaseTarget = chasables.Reduce((target_0 , target_1) => target_0.Score > target_1.Score ? target_0 : target_1);                        
            BTLogger.Log("Number of Chasable Players: " + chasables.Length);          
            return chaseTarget.Transform;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, gizmosChaseRange);
        }
    }
}
