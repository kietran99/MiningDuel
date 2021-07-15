using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    public class Syringe : BaseQuirk
    {
        [SerializeField]
        float HealPercentages = .5f;

        public override void ServerActivate(NetworkIdentity user)
        {
            user.GetComponent<Character.HitPoints>()?.HealPercentageHealth(HealPercentages);
            NetworkServer.Destroy(gameObject);
        }
    }
}
