using UnityEngine;

namespace MD.Character
{
    public class AlwaysGetCounteredZone : WeaponDamageZone
    {
        void Start()
        {
            AttackTime = 0f;
        }
    }
}
