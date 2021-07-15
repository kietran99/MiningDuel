using UnityEngine;

namespace MD.Character
{
    public class AlwaysCounterZone : WeaponDamageZone
    {
        void Start()
        {
            rotatedArc = (counterablePercentage - .1f) * arcMeasure;
            counterableArc = counterablePercentage * arcMeasure;
        }
    }
}
