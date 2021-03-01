using System.Collections.Generic;
using Mirror;

namespace MD.Diggable.Core
{
    public class DiggableEventBroadcaster
    {
        private Dictionary<DiggableType, System.Action<NetworkIdentity, int, int>> eventTriggerDict;
        private DiggableType lastDugType = DiggableType.EMPTY;
        private IDiggableGenerator diggableGenerator;

        public DiggableEventBroadcaster(IDiggableGenerator diggableGenerator)
        {
            this.diggableGenerator = diggableGenerator;
            eventTriggerDict = new Dictionary<DiggableType, System.Action<NetworkIdentity, int, int>>()
            {
                { DiggableType.COMMON_GEM, TriggerGemDugEvent },
                { DiggableType.UNCOMMON_GEM, TriggerGemDugEvent },
                { DiggableType.RARE_GEM, TriggerGemDugEvent },
                { DiggableType.SUPER_RARE_GEM, TriggerGemDugEvent },
                { DiggableType.NORMAL_BOMB, TriggerProjectileDugEvent },
                { DiggableType.EMPTY, (digger, cur, max) => { UnityEngine.Debug.Log("Dug Empty Tile"); } }
            };
        }

        private void TriggerGemDugEvent(NetworkIdentity digger, int cur, int max)
        {
            UnityEngine.Debug.Log("Diggable Generator: " + (diggableGenerator.DigProgressEvent == null));
            diggableGenerator
                .DigProgressEvent?
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

        public void TriggerDiggableSpawnEvent(int x, int y, DiggableType type)
        {
            diggableGenerator.DiggableSpawnEvent?.Invoke(new MD.Diggable.DiggableSpawnData(x, y, type));
        }

        public void TriggerDiggableDestroyEvent(int x, int y)
        {
            diggableGenerator.DiggableDestroyEvent?.Invoke(new MD.Diggable.DiggableRemoveData(x, y));
        }  
    }
}