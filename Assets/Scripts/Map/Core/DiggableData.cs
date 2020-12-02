using UnityEngine;
using System.Collections.Generic;

namespace MD.Map.Core
{
    public class DiggableData : MonoBehaviour, IDiggableData
    {             
        private Dictionary<Vector2Int, ITileData> tiles = new Dictionary<Vector2Int, ITileData>();

        public void Populate((Vector2Int pos, ITileData data)[] tiles)
        {
            tiles.ForEach(tile => this.tiles.Add(tile.pos, tile.data));
        }

        public bool TryGetAt(int x, int y, out ITileData data)
        {
            return tiles.TryGetValue(new Vector2Int(x, y), out data);
        }

        public bool TrySetAt(int x, int y, ITileData data)
        {
            return TryApplyActionAt(x, y, foundData => foundData = data);
        }

        public bool TryReduceAt(int x, int y, int reduceVal)
        {
            return TryApplyActionAt(x, y, data => data.Reduce(reduceVal));
        }

        public void SetAt(int x, int y, ITileData data)
        {
            ApplyActionAt(x, y, foundData => foundData = data);
        }

        public void ReduceAt(int x, int y, int reduceVal)
        {
            ApplyActionAt(x, y, data => data.Reduce(reduceVal));
        }

        public bool IsEmptyAt(int x, int y)
        {            
            if (!TryGetAt(x, y, out ITileData tile)) 
            { 
                throw new InvalidTileException();
            }

            return tile.IsEmpty();
        }
        
        private bool TryApplyActionAt(int x, int y, System.Action<ITileData> action)
        {
            var pos = new Vector2Int(x, y);
            if (tiles.ContainsKey(pos))
            {
                action(tiles[pos]);
                return true;
            }

            return false;
        }

        private void ApplyActionAt(int x, int y, System.Action<ITileData> action)
        {
            var pos = new Vector2Int(x, y);
            if (!tiles.ContainsKey(pos))
            {
                throw new InvalidTileException();               
            }

            action(tiles[pos]);
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