using UnityEngine;

namespace MD.Diggable.Core
{
    public class Map : IMap
    {
        //private Tile[] tiles;

        private TileData[,] tileData;
        private NullTileData nullTile;

        //public Map(Tile[] tiles)
        //{
        //    this.tiles = tiles;
        //    nullTile = new NullTileData();
        //}

        public Map(TileData[,] tileData)
        {
            this.tileData = tileData;
        }

        public bool TryGet(int x, int y, out TileData data)
        {
            try
            {
                data = tileData[x, y];
                return true;
            }
            catch
            {
                data = nullTile;
                return false;
            }
        }

        public bool TrySet(int x, int y, in TileData data)
        {
            try
            {
                tileData[x, y] = data;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Log()
        {
            foreach(var data in tileData)
            {
                Debug.Log(data);
            }
        }

        //public bool TryGet(int x, int y, out TileData data)
        //{
        //    (Tile tile, int idx) = tiles.LookUp(_ => _.X.Equals(x) && _.Y.Equals(y));
        //    data = tile?.Data;
        //    return !idx.Equals(Constants.INVALID);
        //}

        //public bool TrySet(int x, int y, in TileData data)
        //{
        //    (Tile tile, int idx) = tiles.LookUp(_ => _.X.Equals(x) && _.Y.Equals(y));
        //    if (tile != null) tile.Data = data;
        //    return !idx.Equals(Constants.INVALID);
        //}

        //public void Log()
        //{
        //    tiles.ForEach(tile => Debug.Log(tile));
        //    Debug.Log("------------------------------------------");
        //}
    }
}