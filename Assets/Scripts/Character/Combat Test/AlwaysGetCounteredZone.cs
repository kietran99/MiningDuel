using UnityEngine;

namespace MD.Character
{
    public class AlwaysGetCounteredZone : WeaponDamageZone
    {
        void Start()
        {
            rotatedArc = arcMeasure;
            counterableArc = counterablePercentage * arcMeasure;
        }
    }
}
