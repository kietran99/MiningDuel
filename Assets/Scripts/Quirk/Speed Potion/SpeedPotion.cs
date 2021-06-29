using UnityEngine;
using Mirror;
using MD.Character;

namespace MD.Quirk
{
    public class SpeedPotion : BaseQuirk
    {
        [SerializeField]
        float speedPercentage = 0f;
        
        [SerializeField]
        float time = 3f;

        public override void SingleActivate(NetworkIdentity user)
        {
            user.GetComponent<MoveAction>()?.CmdModifySpeed(speedPercentage, time);
            CmdRequestDestroy();
        }

        [Command]
        private void CmdRequestDestroy()
        {
            NetworkServer.Destroy(gameObject);   
        }   
    }
}
