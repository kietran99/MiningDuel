using UnityEngine;

public interface IMapManager
{
    Vector2Int GetMapSize(); 
    bool TrySpawnAt(Vector2Int idx, DiggableType diggable, GameObject prefab);
    bool IsProjectileAt(Vector2 pos);
    bool IsGemAt(Vector2 pos);
}
