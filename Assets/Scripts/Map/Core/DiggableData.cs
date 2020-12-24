using UnityEngine;
using System.Collections.Generic;
using Functional.Type;
using System.Collections.ObjectModel;

namespace MD.Map.Core
{
    public class DiggableData : IDiggableData
    {                     
        private Dictionary<Vector2Int, ITileData> occupiedTiles = new Dictionary<Vector2Int, ITileData>();
        private List<Vector2Int> freeTiles = new List<Vector2Int>();
        private HashSet<IDiggableAccess> accesses = new HashSet<IDiggableAccess>();

        public ReadOnlyCollection<Vector2Int> FreeTiles { get => freeTiles.AsReadOnly(); }
       
        public DiggableData((Vector2Int pos, ITileData data)[] tiles)
        {
            Populate(tiles);
        }

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
                var access = new DiggableAccess(x, y);
                accesses.Add(access); 
                return access; 
            }
            
            return new InvalidTileException();
        }

        public void SetData(IDiggableAccess access, ITileData data)
        {
            occupiedTiles[new Vector2Int(access.X, access.Y)] = data;
        }

        //TODO Check if tile is empty
        public void Spawn(IDiggableAccess access, DiggableType type)
        {
            if (!ValidateAccess(access)) return;

            var pos = new Vector2Int(access.X, access.Y);
            freeTiles.Remove(pos);
            occupiedTiles[pos] = new TileData(type);
        }

        public void Reduce(IDiggableAccess access, int reduceVal)
        {
            if (!ValidateAccess(access)) return;

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

        public Either<InvalidTileException, ITileData> GetDataAt(int x, int y)
        {
            if (occupiedTiles.TryGetValue(new Vector2Int(x, y), out ITileData data))
            {
                return (Either<InvalidTileException, ITileData>) data;
            }

            return new InvalidTileException();
        }

        private bool ValidateAccess(IDiggableAccess access)
        {
            bool valid = accesses.Contains(access);

            if (!valid)
            {
                Debug.LogError("Illegal Diggable Access");
            }

            return valid;
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