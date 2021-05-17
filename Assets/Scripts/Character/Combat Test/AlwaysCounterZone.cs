using UnityEngine;

namespace MD.Character
{
    public class AlwaysCounterZone : WeaponDamageZone
    {
        void Start()
        {
            attackTime = 100f;
        }
    }
}
