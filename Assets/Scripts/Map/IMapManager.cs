﻿using UnityEngine;
using Mirror;

public interface IMapManager
{
    void DigAtPosition(NetworkIdentity player);
    ScanAreaData GetScanAreaData(Vector2[] posToScan);
    Vector2Int GetMapSize(); 
    Vector2Int PositionToIndex(Vector2 position);
    bool TrySpawnDiggableAtIndex(Vector2Int idx, DiggableType diggable, GameObject prefab);
    void NotifyNewGem(Vector2 pos, DiggableType diggable);
    bool IsProjectileAt(Vector2 pos);
    bool IsGemAt(Vector2 pos);
}
