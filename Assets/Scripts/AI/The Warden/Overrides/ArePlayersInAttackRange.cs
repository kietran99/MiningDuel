using System.Collections.Generic;
using Functional.Type;
using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ArePlayersInAttackRange : BTLeaf
    {
        private class AttackTarget
        {
            private Transform transform;
            private IWardenDamagable damagable;

            public AttackTarget(Transform transform, IWardenDamagable damagable)
            {
                this.transform = transform;
                this.damagable = damagable;
            }

            public static Option<AttackTarget> New(Transform transform)
            {
                var mayDamagable = transform.GetComponent<IWardenDamagable>();
                if (mayDamagable == null)
                {
                    Debug.LogError("No " + typeof(IWardenDamagable) + " derived script attached to this GameObject");
                    return Option<AttackTarget>.None;
                }

                return new AttackTarget(transform, mayDamagable);
            }

            public Transform Transform => transform;

            public Vector3 Position => transform.position;

            public IWardenDamagable Damagable => damagable;
        }

        [SerializeField]
        private float baseAttackRange = 2f;

        private readonly float ATTACKABLE_DIST = .2f;

        private AttackTarget[] allTargets;

        private Vector3 gzmLastActorPos;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            blackboard
                .Get<Transform[]>(WardenMacros.PLAYERS)
                .Match(
                    players => 
                    {
                        var maybeValidTargets = players.Map(player => AttackTarget.New(player));
                        if (maybeValidTargets.Length != players.Length)
                        {
                            return;
                        }

                        var targets = new List<AttackTarget>();
                        maybeValidTargets.ForEach(maybeTarget => maybeTarget.Match(target => targets.Add(target), () => {}));
                        allTargets = targets.ToArray();
                    },
                    () => gameObject.SetActive(false)
                );

            EventSystems.EventManager.Instance.StartListening<Character.CharacterDeathData>(FilterRemainingPlayers);
        }

        private void FilterRemainingPlayers(Character.CharacterDeathData data)
        {
            allTargets = allTargets.Filter(target => target.Transform.GetComponent<Mirror.NetworkIdentity>().netId != data.eliminatedId);

            bool matchEnded = allTargets.Length == 1;
            if (matchEnded)
            {
                EventSystems.EventManager.Instance.StopListening<Character.CharacterDeathData>(FilterRemainingPlayers);
            }
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            if (allTargets.Length == 0)
            {
                return BTNodeState.FAILURE;
            }

            var attackable = allTargets.Find(target => (actor.transform.position - target.Position).sqrMagnitude <= ATTACKABLE_DIST);
            
            gzmLastActorPos = actor.transform.position;

            if (!attackable.HasValue)
            {
                return BTNodeState.FAILURE;
            }

            var attackableTargets = allTargets.Filter(target => (actor.transform.position - target.Position).sqrMagnitude <= baseAttackRange * baseAttackRange);

            blackboard.Set<IWardenDamagable[]>(WardenMacros.DAMAGABLES, attackableTargets.Map(target => target.Damagable));
            return BTNodeState.SUCCESS;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gzmLastActorPos, baseAttackRange);
        }
    }
}
