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

        public void SetAt(int x, int y, ITileData data)
        {
            tiles[GetPosition(x, y)] = data;
        }

        public void ReduceAt(int x, int y, int reduceVal)
        {
            tiles[GetPosition(x, y)].Reduce(reduceVal);
        }

        public bool IsEmptyAt(int x, int y)
        {            
            if (!TryGetAt(x, y, out ITileData tile)) 
            { 
                throw new InvalidTileException();
            }

            return tile.IsEmpty();
        }
               
        private Vector2Int GetPosition(int x, int y)
        {
            var pos = new Vector2Int(x, y);
            if (!tiles.ContainsKey(pos))
            {
                throw new InvalidTileException();               
            }

            return pos;
        }

        public void Log()
        {
            Debug.Log("<----------------DIGGABLE DATA---------------->");

            foreach (var tile in tiles)
            {
                if (tile.Value.Type.Equals(DiggableType.Empty))
                {
                    Debug.Log(tile.Key + " " + tile.Value.Type);
                    continue;
                }

                Debug.Log(tile.Key + " " + tile.Value);
            }
            
            Debug.Log("<--------------------------------------------->");
        }       
    }
}