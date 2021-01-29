using UnityEngine;
using Functional.Type;

namespace MD.Map.Core
{
    public interface IDiggableGenerator
    {
        void SetTile(Vector2Int pos, DiggableType type);
        void Populate(Vector2Int[] tilePositions);
        Either<InvalidTileError, Either<InvalidAccessError, ReducedData>> DigAt(int x, int y, int power);
        DiggableType[] GetDiggableArea(Vector2Int[] positions);
    }
}
