using System.Collections.Generic;
using Mirror;
using EventSystems;

namespace MD.Map.Core
{
    public class DiggableEventBroadcaster
    {
        private Dictionary<DiggableType, System.Action<NetworkIdentity, int, int>> eventTriggerDict;
        private DiggableType lastDugType = DiggableType.Empty;
        private IDiggableGenerator diggableGenerator;

        public DiggableEventBroadcaster(IDiggableGenerator diggableGenerator)
        {
            this.diggableGenerator = diggableGenerator;
            eventTriggerDict = new Dictionary<DiggableType, System.Action<NetworkIdentity, int, int>>()
            {
                { DiggableType.CommonGem, TriggerGemDugEvent },
                { DiggableType.UncommonGem, TriggerGemDugEvent },
                { DiggableType.RareGem, TriggerGemDugEvent },
                { DiggableType.NormalBomb, TriggerProjectileDugEvent },
                { DiggableType.Empty, (digger, cur, max) => { UnityEngine.Debug.Log("Dug Empty Tile"); } }
            };
        }

        private void TriggerGemDugEvent(NetworkIdentity digger, int cur, int max)
        {
            diggableGenerator.DigProgressEvent?
                .Invoke(digger.connectionToClient, new MD.Diggable.Gem.DigProgressData(cur, max));

            if (cur <= 0) 
            {
                diggableGenerator.GemObtainEvent?
                    .Invoke(digger.connectionToClient, new MD.Diggable.Gem.GemObtainData(digger.netId, max));
            }         
        }

        private void TriggerProjectileDugEvent(NetworkIdentity digger, int cur, int max)
        {
            diggableGenerator.ProjectileObtainEvent?
                .Invoke(digger.connectionToClient, new MD.Diggable.Projectile.ProjectileObtainData(digger, lastDugType));
        } 

        public void TriggerDiggableDugEvent(NetworkIdentity digger, ReducedData reducedData)
        {     
            lastDugType = reducedData.type;     
            eventTriggerDict[lastDugType](digger, reducedData.current, reducedData.max);            
        } 

        public void TriggerDiggableDestroyEvent(int x, int y)
        {
            diggableGenerator.DiggableDestroyEvent?.Invoke(new MD.Diggable.DiggableRemoveData(x, y));
        }  
    }

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

        private static DiggableType dugType = DiggableType.Empty;

        private static void TriggerGemDugEvent(NetworkIdentity digger, int cur, int max)
        {
            EventManager.Instance.TriggerEvent(new MD.Diggable.Gem.DigProgressData(cur, max));

            if (cur <= 0) 
            {
                EventManager.Instance.TriggerEvent(new MD.Diggable.Gem.GemObtainData(digger.netId, max));
            }         
        }

        private static void TriggerProjectileDugEvent(NetworkIdentity digger, int cur, int max)
        {
            EventManager.Instance.TriggerEvent(new MD.Diggable.Projectile.ProjectileObtainData(digger, dugType));
        } 

        public static void TriggerDiggableDugEvent(NetworkIdentity digger, ReducedData reducedData)
        {     
            dugType = reducedData.type;     
            eventTriggerDict[dugType](digger, reducedData.current, reducedData.max);            
        } 

        public static void TriggerDiggableDestroyEvent(int x, int y)
        {
            EventManager.Instance.TriggerEvent(new MD.Diggable.DiggableRemoveData(x, y));
        }  
    }
}