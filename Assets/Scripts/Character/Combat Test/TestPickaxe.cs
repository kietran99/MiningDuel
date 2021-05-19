using MD.Character;
using UnityEngine;

public class TestPickaxe : MonoBehaviour
{
    [SerializeField]
    private PickaxeAnimatorController pickaxe = null;

    [SerializeField]
    private WeaponDamageZone damageZone = null;

    [SerializeField]
    private PickaxeAnimatorController counteredPickaxe = null;

    [SerializeField]
    private WeaponDamageZone counteredDamageZone = null;

    [SerializeField]
    private float counteredStartTime = .2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            pickaxe.Play();
            damageZone.AttemptSwing();
            Invoke(nameof(DelaySwing), counteredStartTime);
        }
    }

    private void DelaySwing() 
    {
        counteredPickaxe.Play();
        counteredDamageZone.AttemptSwing();
    }
}
