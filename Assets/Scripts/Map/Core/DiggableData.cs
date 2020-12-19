using UnityEngine;
using System.Collections.Generic;
using Functional.Type;

namespace MD.Map.Core
{
    public class DiggableData : MonoBehaviour, IDiggableData
    {             
        private Dictionary<Vector2Int, ITileData> occupiedTiles = new Dictionary<Vector2Int, ITileData>();
        private List<Vector2Int> freeTiles = new List<Vector2Int>();

        public List<Vector2Int> FreeTiles { get => freeTiles; }
       
        public void Populate((Vector2Int pos, ITileData data)[] tiles)
        {
            tiles.ForEach(AddToOccupiedAndOrFreeList);
        }

        private void AddToOccupiedAndOrFreeList((Vector2Int pos, ITileData data) tile)
        {
            occupiedTiles.Add(tile.pos, tile.data);
            if (tile.data.Type.Equals(DiggableType.Empty))
            {
                freeTiles.Add(tile.pos);
            }
        }

        public Either<InvalidTileException, IDiggableAccess> GetAccessAt(int x, int y)
        {
            if (occupiedTiles.ContainsKey(new Vector2Int(x, y)))
            {
                //return DiggableAccess.Create(this);
                return new DiggableAccess(x, y);
            }

            return new InvalidTileException();
        }

        public void SetData(IDiggableAccess access, ITileData data)
        {
            // occupiedTiles[GetPosition(access.X, access.Y)] = data;
            occupiedTiles[new Vector2Int(access.X, access.Y)] = data;
        }

        public void Reduce(IDiggableAccess access, int reduceVal)
        {
            //var pos = GetPosition(x, y);
            var pos = new Vector2Int(access.X, access.Y);
            occupiedTiles[pos].Reduce(reduceVal, out bool isEmpty);
            if (isEmpty) { freeTiles.Add(pos); }  
        }

        public Either<InvalidTileException, bool> IsEmptyAt(int x, int y)
        {   
            if (occupiedTiles.TryGetValue(new Vector2Int(x, y), out ITileData data))
            {
                return data.IsEmpty();
            }

            return new InvalidTileException();            
        }

        public Either<InvalidTileException, ITileData> TryGetAt(int x, int y)
        {
            if (occupiedTiles.TryGetValue(new Vector2Int(x, y), out ITileData data))
            {
                return (Either<InvalidTileException, ITileData>) data;
            }

            return new InvalidTileException();
        }

        private Either<Vector2Int, InvalidTileException> GetPosition(int x, int y)
        {
            var pos = new Vector2Int(x, y);
            if (!occupiedTiles.ContainsKey(pos))
            {
                return new InvalidTileException();            
            }

            return pos;
        }

        public void Log()
        {
            Debug.Log("<----------------DIGGABLE DATA---------------->");



            Debug.Log("------------OCCUPIED TILES------------");
            foreach (var tile in occupiedTiles)
            {
                if (!tile.Value.Type.Equals(DiggableType.Empty))
                {
                    Debug.Log(tile.Key + " " + tile.Value);
                }                
            }
            Debug.Log("--------------------------------------");




            Debug.Log("--------------FREE TILES--------------");
            freeTiles.ForEach(_ => Debug.Log(_));
            Debug.Log("--------------------------------------");


            
            Debug.Log("<--------------------------------------------->");
        }       
    }
}