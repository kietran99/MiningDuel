using System;
using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public abstract class WardenRoot : BTRoot
    {
        protected abstract Transform[] Players { get; }

        protected abstract (Vector2, Vector2) MapBounds { get; }

        protected abstract IWardenParticleController ParticleController { get; }

        protected abstract IWardenAttackChargeIndicator AtkChargeIndicator { get; }

        protected override void SetupAdditionalStates(BTBlackboard blackboard)
        {
            blackboard.Set(WardenMacros.PLAYERS, Players); 
            blackboard.Set(WardenMacros.ATTACK_COOLDOWN, 0f);
            blackboard.Set(WardenMacros.QUADRANTS, MakeQuadrant(MapBounds.Item1, MapBounds.Item2));
            blackboard.Set(WardenMacros.PARTICLE_CONTROLLER, ParticleController);
            blackboard.Set(WardenMacros.ANIMATOR, actor.GetComponentInChildren<Animator>());
            blackboard.Set(WardenMacros.DELTA_CHASE_RANGE, 0f);   
            blackboard.Set(WardenMacros.SHOULD_CHARGE_ATK, false);
            blackboard.Set(WardenMacros.ATK_CHARGE_INDICATOR, AtkChargeIndicator);
        }

        protected Quadrant[] MakeQuadrant(Vector2 topRightLimit, Vector2 botLeftLimit)
        {
            Func<Vector2, float, bool> IsNearTopLimit = (pos, moveDist) => pos.y + moveDist >= topRightLimit.y;
            Func<Vector2, float, bool> IsNearBotLimit = (pos, moveDist) => pos.y - moveDist <= botLeftLimit.y;
            Func<Vector2, float, bool> IsNearRightLimit = (pos, moveDist) => pos.x + moveDist >= topRightLimit.x;
            Func<Vector2, float, bool> IsNearLeftLimit = (pos, moveDist) => pos.x - moveDist <= botLeftLimit.x; 

            return new Quadrant[4]
            {
                new Quadrant(0, 90, new Func<Vector2, float, bool>[2] { IsNearTopLimit, IsNearRightLimit }),
                new Quadrant(90, 180, new Func<Vector2, float, bool>[2] { IsNearTopLimit, IsNearLeftLimit }),
                new Quadrant(180, 270, new Func<Vector2, float, bool>[2] { IsNearBotLimit, IsNearLeftLimit }),
                new Quadrant(270, 360, new Func<Vector2, float, bool>[2] { IsNearBotLimit, IsNearRightLimit })
            };
        }
    }
}
