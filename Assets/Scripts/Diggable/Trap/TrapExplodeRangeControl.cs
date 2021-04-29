using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class TrapExplodeRangeControl : MonoBehaviour
{
    [SerializeField]
    LinkedTrap trap;

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
                if (otherTrap.GetOwner().Equals(trap.GetOwner())) //  only link traps of the same owner
                {
                    otherTrap.RegistLinkedTrap(trap);
                    trap.RegistLinkedTrap(otherTrap);
                }
                else
                {
                    Debug.Log(this.trap.GetOwner());
                }
            }
            else
            {
                Debug.LogError("Cant get trapexploderangecontrol script");
            }
        }
    }
    public LinkedTrap GetTrap() => trap;

}
