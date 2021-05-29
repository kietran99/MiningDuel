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
        
        private ChaseTarget[] targets;

        private Vector3 gzmLastActorPos;
        private float gzmChaseRange;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            blackboard
                .Get<Transform[]>(WardenMacros.PLAYERS)
                .Match(
                    players => 
                    {
                        var maybeValidTargets = players.Map(player => ChaseTarget.New(player));
                        if (maybeValidTargets.Length != players.Length)
                        {
                            return;
                        }

                        var targets = new System.Collections.Generic.List<ChaseTarget>();
                        maybeValidTargets.ForEach(maybeTarget => maybeTarget.Match(target => targets.Add(target), () => {}));
                        this.targets = targets.ToArray();
                        gzmChaseRange = baseChaseRange;
                    },
                    () => gameObject.SetActive(false)
                );

            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<Character.CharacterDeathData>(FilterRemainingPlayers);
        }

        private void FilterRemainingPlayers(Character.CharacterDeathData data)
        {
            targets = targets.Filter(target => target.Transform.GetComponent<Mirror.NetworkIdentity>().netId != data.eliminatedId);
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            if (targets.Length == 0)
            {
                return BTNodeState.FAILURE;
            }

            var chaseRange = blackboard
                .Get<float>(WardenMacros.DELTA_CHASE_RANGE, true)
                .Match(
                    deltaChaseRange => baseChaseRange + deltaChaseRange,
                    () => baseChaseRange
                ); 

            gzmChaseRange = chaseRange;
            gzmLastActorPos = actor.transform.position;

            var maybeTarget = CheckTargetsInRange(actor.transform.position, targets, chaseRange);   

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

        private Option<Transform> CheckTargetsInRange(Vector3 actorPos, ChaseTarget[] targets, float range)
        {
            var possibleTargets = targets.Filter(target => (actorPos - target.Position).sqrMagnitude <= range * range);

            if (possibleTargets.Length == 0)
            {
                return Option<Transform>.None;
            }
                    
            var chosenTarget = possibleTargets.Reduce((target_0 , target_1) => CalcScore(actorPos, target_0) > CalcScore(actorPos, target_1) ? target_0 : target_1);                        
            BTLogger.Log("Number of Chasable Players: " + possibleTargets.Length);          
            return chosenTarget.Transform;
        }

        private float CalcScore(Vector3 actorPos, ChaseTarget target)
        {
            var MULT = 1000f;
            return target.Score * MULT / (actorPos - target.Position).sqrMagnitude;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gzmLastActorPos, gzmChaseRange);
        }
    }
}
