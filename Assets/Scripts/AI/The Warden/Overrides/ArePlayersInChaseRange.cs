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
        private float lockTargetDistance = 1.6f;
        
        private ChaseTarget[] targets;
        private System.Collections.Generic.List<ChaseTarget> possibleTargets;

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

                        gzmChaseRange = baseChaseRange;

                        var targets = new System.Collections.Generic.List<ChaseTarget>();
                        maybeValidTargets.ForEach(maybeTarget => maybeTarget.Match(target => targets.Add(target), () => {}));
                        this.targets = targets.ToArray();
                        this.possibleTargets = new System.Collections.Generic.List<ChaseTarget>(players.Length);
                        this.possibleTargets.TrimExcess();
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

            var chaseRange = baseChaseRange + blackboard.NullableGet<float>(WardenMacros.DELTA_CHASE_RANGE, true);

            gzmChaseRange = chaseRange;
            gzmLastActorPos = actor.transform.position;

            if (!CheckTargetsInRange(actor.transform.position, targets, chaseRange, out var chaseTarget))
            {
                return BTNodeState.FAILURE;
            }

            blackboard.Set(WardenMacros.CHASE_TARGET, chaseTarget);
            return BTNodeState.SUCCESS;   
            // return BTNodeState.FAILURE;   
        }
        
        private bool CheckTargetsInRange(Vector3 actorPos, ChaseTarget[] allTargets, float range, out Transform chaseTarget)
        {
            possibleTargets.Clear();
        
            for (int i = 0; i < allTargets.Length; i++)
            {
                if ((actorPos - allTargets[i].Position).sqrMagnitude <= range * range)
                {
                    possibleTargets.Add(allTargets[i]);
                }
            }

            if (possibleTargets.Count == 0)
            {
                chaseTarget = null;
                return false;
            }
                    
            // Debug.Log("Number of Chasable Players: " + possibleTargets.Length);         
            
            ChaseTarget chosenTarget = possibleTargets[0];

            for (int i = 1; i < possibleTargets.Count; i++)
            {
                chosenTarget = 
                    CalcScore(actorPos, possibleTargets[i - 1]) > CalcScore(actorPos, possibleTargets[i]) 
                    ? possibleTargets[i - 1] 
                    : possibleTargets[i];
            }

            chaseTarget = chosenTarget.Transform;
            return true;
        }

        private float CalcScore(Vector3 actorPos, ChaseTarget target)
        {
            var targetDist = (actorPos - target.Position).sqrMagnitude;

            if (targetDist <= lockTargetDistance)
            {
                return Mathf.Infinity;
            }

            int getMult(int val, int accum = 1)
            {
                return accum < val ? getMult(val, accum * 10) : accum;
            }     

            return target.Score - getMult(Mathf.FloorToInt(target.Score / targetDist)) * (targetDist);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gzmLastActorPos, gzmChaseRange);
        }
    }
}
