﻿using UnityEngine;

namespace MD.Character
{
    public class AlwaysCounterZone : WeaponDamageZone
    {
        void Start()
        {
            AttackTime = 100f;
        }
    }
}
