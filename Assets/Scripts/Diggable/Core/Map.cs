using UnityEngine;
using System.Collections.Generic;

namespace MD.Map.Core
{
    public class Map : IMap
    {               
        private Dictionary<Vector2, TileData> tiles = new Dictionary<Vector2, TileData>();
       
        public Map((Vector2 pos, TileData data)[] tiles)
        {
            tiles.ForEach(tile => this.tiles.Add(tile.pos, tile.data));
        }

        public bool TryGetAt(int x, int y, out TileData data)
        {
            return tiles.TryGetValue(new Vector2(x, y), out data);
        }

        public bool TrySetAt(int x, int y, in TileData data)
        {
            var inputData = data;
            return TryApplyActionAt(x, y, foundData => foundData = inputData);
        }

        public bool TryReduceAt(int x, int y, int reduceVal)
        {
            return TryApplyActionAt(x, y, data => data.Reduce(reduceVal));
        }

        public bool IsEmptyAt(int x, int y)
        {
            var pos = new Vector2(x, y);
            if (tiles.ContainsKey(pos))
            {
                return tiles[pos].IsEmpty();
            }

            return false;
        }

        private bool TryApplyActionAt(int x, int y, System.Action<TileData> action)
        {
            var pos = new Vector2(x, y);
            if (tiles.ContainsKey(pos))
            {
                action(tiles[pos]);
                return true;
            }

            return false;
        }

        public void Log()
        {
            Debug.Log("------------------------------------------");

            foreach (var tile in tiles)
            {
                Debug.Log(tile.Key + " " + tile.Value);
            }
            
            Debug.Log("------------------------------------------");
        }
    }
}