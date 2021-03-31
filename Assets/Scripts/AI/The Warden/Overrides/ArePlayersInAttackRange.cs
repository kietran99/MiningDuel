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
        private Transform[] players = null; // For testing targets

        [SerializeField]
        private float baseAttackRange = 2f;

        private AttackTarget[] allTargets;

        private void Start()
        {
            var maybeValidTargets = players.Map(player => AttackTarget.New(player));
            if (maybeValidTargets.Length != players.Length)
            {
                return;
            }

            var targets = new System.Collections.Generic.List<AttackTarget>();
            maybeValidTargets.ForEach(maybeTarget => maybeTarget.Match(target => targets.Add(target), () => {}));
            allTargets = targets.ToArray();
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var attackableTargets = allTargets.Filter(target => (actor.transform.position - target.Position).sqrMagnitude <= baseAttackRange * baseAttackRange);
            
            if (attackableTargets.Length == 0)
            {
                return BTNodeState.FAILURE;
            }

            blackboard.Set<IWardenDamagable[]>(WardenMacros.DAMAGABLES, attackableTargets.Map(target => target.Damagable));
            return BTNodeState.SUCCESS;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, baseAttackRange);
        }
    }
}
