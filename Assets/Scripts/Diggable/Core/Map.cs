using UnityEngine;

namespace MD.Diggable.Core
{
    public class Map : IMap
    {
        private Tile[] tiles;

        public Map(Tile[] tiles)
        {
            this.tiles = tiles;
        }

        public bool TryGet(int x, int y, out TileData data)
        {
            (Tile tile, int idx) = tiles.LookUp(_ => _.X.Equals(x) && _.Y.Equals(y));
            data = tile?.Data;
            return !idx.Equals(Constants.INVALID);
        }

        public bool TrySet(int x, int y, in TileData data)
        {
            (Tile tile, int idx) = tiles.LookUp(_ => _.X.Equals(x) && _.Y.Equals(y));
            if (tile != null) tile.Data = data;
            return !idx.Equals(Constants.INVALID);
        }

        public void Log()
        {
            tiles.ForEach(tile => Debug.Log(tile));
            Debug.Log("------------------------------------------");
        }
    }
}