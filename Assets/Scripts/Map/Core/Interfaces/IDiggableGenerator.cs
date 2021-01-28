using UnityEngine;

namespace MD.Map.Core
{
    public interface IDiggableGenerator
    {
        void SetTile(Vector2Int pos, DiggableType type);
        void Populate(Vector2Int[] tilePositions);
        void DigAt(int x, int y, int power, uint diggerID);
        DiggableType[] GetDiggableArea(Vector2Int[] positions);
    }
}
