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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pickaxe.Play();
            damageZone.AttemptSwing();
            if (counteredDamageZone != null)
            {
                Invoke(nameof(DelaySwing), counteredStartTime);
            }
        }
    }

    private void DelaySwing() 
    {
        counteredPickaxe.Play();
        counteredDamageZone.AttemptSwing();
    }
}
