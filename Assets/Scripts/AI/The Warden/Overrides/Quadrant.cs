using System;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class Quadrant
    {
        private int minAngle, maxAngle;
        private Func<Vector2, float, bool>[] limitCheckers;

        public Quadrant(int min, int max, Func<Vector2, float, bool>[] limitCheckers)
        {
            this.minAngle = min;
            this.maxAngle = max;
            this.limitCheckers = limitCheckers;
        }

        public bool IsIn(int angle) => minAngle <= angle && angle < maxAngle;

        public bool Movable(Vector2 pos, float moveDist) => !limitCheckers.Find(checker => checker(pos, moveDist)).HasValue;

        public int RandAngle() => UnityEngine.Random.Range(minAngle, maxAngle);
    }
}


