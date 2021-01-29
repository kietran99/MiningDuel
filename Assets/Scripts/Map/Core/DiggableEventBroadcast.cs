using System.Collections.Generic;
using Mirror;
using EventSystems;

namespace MD.Map.Core
{
    public static class DiggableEventBroadcast
    {
        private static Dictionary<DiggableType, System.Action<NetworkIdentity, int, int>> eventTriggerDict = 
            new Dictionary<DiggableType, System.Action<NetworkIdentity, int, int>>()
            {
                { DiggableType.CommonGem, TriggerGemDugEvent },
                { DiggableType.UncommonGem, TriggerGemDugEvent },
                { DiggableType.RareGem, TriggerGemDugEvent },
                { DiggableType.NormalBomb, TriggerProjectileDugEvent },
                { DiggableType.Empty, (digger, cur, max) => { UnityEngine.Debug.Log("Dug Empty Tile"); } }
            };

        private static void TriggerGemDugEvent(NetworkIdentity digger, int cur, int max)
        {
            // Debug.Log("Trigger Gem Dug Event");
            EventManager.Instance.TriggerEvent(new MD.Diggable.Gem.DigProgressData(cur, max));

            if (cur <= 0) 
            {
                EventManager.Instance.TriggerEvent(new MD.Diggable.Gem.GemDugData(digger.netId, max));
            }         
        }

        private static void TriggerProjectileDugEvent(NetworkIdentity digger, int cur, int max)
        {
            UnityEngine.Debug.Log("Trigger Projectile Dug Event");
        } 

        public static void TriggerDiggableDugEvent(NetworkIdentity digger, ReducedData reducedData)
        {
            eventTriggerDict[reducedData.type](digger, reducedData.current, reducedData.max);            
        } 

        public static void TriggerDiggableDestroyEvent(int x, int y)
        {
            EventManager.Instance.TriggerEvent(new MD.Diggable.DiggableRemoveData(x, y));
        }  
    }
}