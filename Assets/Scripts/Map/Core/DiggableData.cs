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

        public Option<InvalidTileException> SetAt(int x, int y, ITileData data)
        {
            //occupiedTiles[GetPosition(x, y)] = data;
            Either<Vector2Int, InvalidTileException> pos = GetPosition(x, y);
            Option<InvalidTileException> isTileValid = new Option<InvalidTileException>();
            pos.Match(
                validPos => occupiedTiles[validPos] = data,
                invalidTileException => isTileValid = new InvalidTileException()
            );

            return isTileValid;
        }

        public void ReduceAt(int x, int y, int reduceVal)
        {
            var pos = GetPosition(x, y);
            //occupiedTiles[pos].Reduce(reduceVal, out bool isEmpty)); 
            //if (isEmpty) { freeTiles.Add(pos); }                  
        }

        public Either<bool, InvalidTileException> IsEmptyAt(int x, int y)
        {     
            if (!TryGetAt(x, y, out ITileData tile)) 
            { 
                //throw new InvalidTileException();
                return new InvalidTileException();
            }

            return tile.IsEmpty();
        }

        public bool TryGetAt(int x, int y, out ITileData data)
        {
            return occupiedTiles.TryGetValue(new Vector2Int(x, y), out data);
        }

        private Either<Vector2Int, InvalidTileException> GetPosition(int x, int y)
        {
            var pos = new Vector2Int(x, y);
            if (!occupiedTiles.ContainsKey(pos))
            {
                //throw new InvalidTileException();   
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