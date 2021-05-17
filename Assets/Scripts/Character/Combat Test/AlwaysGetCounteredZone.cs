using UnityEngine;

namespace MD.Character
{
    public class AlwaysGetCounteredZone : WeaponDamageZone
    {
        void Start()
        {
            attackTime = 0f;
        }
    }
}
