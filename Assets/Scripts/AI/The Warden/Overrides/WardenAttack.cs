using MD.AI.BehaviourTree;
using MD.Character;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenAttack : BTLeaf
    {
        [SerializeField]
        private int power = 40;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<IWardenDamagable[]>(WardenMacros.DAMAGABLES)
                    .Map(
                        damagables => 
                        {
                            damagables.ForEach(damagable => damagable.TakeWardenDamage(power));
                            return BTNodeState.SUCCESS;
                        }
                    );
        }
    }
}
