using MD.UI;
using Mirror;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class NetWardenRoot : WardenRoot
    {
        protected override Transform[] Players
        {
            get
            {
                var netManager = NetworkManager.singleton as NetworkManagerLobby;
                return netManager.Players.Map(player => player.transform);
            }
        }

        protected override (Vector2, Vector2) MapBounds => 
            (new Vector3(29f, 29f, 0f) - new Vector3(.5f, .5f, 0f), new Vector3(0f, 0f, 0f) + new Vector3(.5f, .5f, 0f));
    }
}
