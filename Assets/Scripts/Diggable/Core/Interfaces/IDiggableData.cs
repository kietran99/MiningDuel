using UnityEngine;
using Functional.Type;
using System.Collections.ObjectModel;

namespace MD.Diggable.Core
{
    /// <summary>
    /// Manages in-game map and tile data
    /// </summary>
    public interface IDiggableData
    {
        ReadOnlyCollection<Vector2Int> FreeTiles { get; }

        /// <summary>
        /// Populate with tile data
        /// </summary>
        void Populate((UnityEngine.Vector2Int pos, ITileData data)[] tiles);



        /// <summary>
        /// Get tile data at position ( <paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="data">Output tile data</param>
        /// <returns>Whether a tile at ( <paramref name="x"/>, <paramref name="y"/>) is valid</returns>
        Either<InvalidTileError, ITileData> GetDataAt(int x, int y);



        /// <summary>
        /// Set tile data at position ( <paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="data">Input tile data</param>
        /// <exception cref="MD.Map.Core.InvalidTileError"></exception>
        void SetData(IDiggableAccess access, ITileData data);



        Either<InvalidTileError, IDiggableAccess> GetAccessAt(int x, int y);



        void Spawn(IDiggableAccess access, DiggableType type);



        /// <summary>
        /// Reduce remaining dig(s) at position ( <paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="reduceVal">Reduce value</param>
        /// <exception cref="MD.Map.Core.InvalidTileError"></exception>
        Either<InvalidAccessError, ReducedData> Reduce(IDiggableAccess access, int reduceVal);



        /// <summary>
        /// Check whether position ( <paramref name="x"/>, <paramref name="y"/>) has no diggable
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <exception cref="MD.Map.Core.InvalidTileError"></exception>
        Either<InvalidTileError, bool> IsEmptyAt(int x, int y); 


  
        /// <summary>
        /// Logging method for debugging purpose
        /// </summary>    
        void Log();
    }
}