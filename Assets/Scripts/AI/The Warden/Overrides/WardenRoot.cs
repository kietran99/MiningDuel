using System;
using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public abstract class WardenRoot : BTRoot
    {
        protected abstract Transform[] Players { get; }

        protected abstract (Vector2, Vector2) MapBounds { get; }

        protected override void SetupAdditionalStates(BTBlackboard blackboard)
        {
            blackboard.Set<Transform[]>(WardenMacros.PLAYERS, Players); 
            blackboard.Set<float>(WardenMacros.ATTACK_COOLDOWN, 0f);
            blackboard.Set<Quadrant[]>(WardenMacros.QUADRANTS, MakeQuadrant(MapBounds.Item1, MapBounds.Item2));
            blackboard.Set<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER, actor.GetComponent<IWardenParticleController>());
            blackboard.Set<Animator>(WardenMacros.ANIMATOR, actor.GetComponentInChildren<Animator>());
            blackboard.Set<float>(WardenMacros.DELTA_CHASE_RANGE, 0);   
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
