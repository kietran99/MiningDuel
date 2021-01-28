using UnityEngine;
using Mirror;

public interface IMapManager
{
    void DigAt(NetworkIdentity player, Vector2 position);
    ScanAreaData GetScanAreaData(Vector2Int[] posToScan);
    Vector2Int GetMapSize(); 
    bool TrySpawnAt(Vector2Int idx, DiggableType diggable, GameObject prefab);
    bool IsProjectileAt(Vector2 pos);
    bool IsGemAt(Vector2 pos);
}
