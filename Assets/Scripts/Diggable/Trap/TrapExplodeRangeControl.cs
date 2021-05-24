using System.Collections.Generic;
using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class TrapExplodeRangeControl : MonoBehaviour
    {
        [SerializeField]
        LinkedTrap trap = null;

        [SerializeField]
        ContactFilter2D filterMask;

        public void LinkNearbyTraps()
        {
            Collider2D collider = GetComponent<Collider2D>();
            List<Collider2D> results = new List<Collider2D>();
            collider.OverlapCollider(filterMask, results);
            for (int i=0; i < results.Count; i++)
            {
                TrapExplodeRangeControl trapCollider = results[i].GetComponent<TrapExplodeRangeControl>();
                if (trapCollider != null)
                {
                    LinkedTrap otherTrap = trapCollider.GetTrap();
                    otherTrap.RegistLinkedTrap(trap, false);
                    trap.RegistLinkedTrap(otherTrap, otherTrap.GetOwner().Equals(trap.GetOwner())); //create wire on traps of the same owner
                }
                else
                {
                    Debug.LogError("Cant get trapexploderangecontrol script");
                }
            }
        }

        public LinkedTrap GetTrap() => trap;

    }
}
