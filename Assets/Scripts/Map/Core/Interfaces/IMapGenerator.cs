using System.Collections.Generic;
using UnityEngine;

namespace MD.Map.Core
{
    public interface IMapGenerator
    {
        int MapWidth { get; }
        int MapHeight { get; }
        SpawnPositionsData SpawnPositionsData { get; }
        List<Vector2Int> MovablePostions { get; }
        int[] MapData { get; }
        bool IsObstacle(int x, int y);
        bool UseGeneratedMaps {get;}
        string mapUsed{ get;}
        List<Vector3> SpawnStoragePos();
    }
}