using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenAttack : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<IWardenDamagable[]>(WardenMacros.DAMAGABLES)
                    .Match(
                        damagables => 
                        {
                            damagables.ForEach(damagable => damagable.TakeDamage());
                            return BTNodeState.SUCCESS;
                        },
                        () => BTNodeState.FAILURE
                    );
        }
    }
}
