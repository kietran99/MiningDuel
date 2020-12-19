﻿using UnityEngine;
using Functional.Type;

namespace MD.Map.Core
{
    /// <summary>
    /// Manages in-game map and tile data
    /// </summary>
    public interface IDiggableData
    {
        System.Collections.Generic.List<Vector2Int> FreeTiles { get; }

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
        Either<InvalidTileException, ITileData> TryGetAt(int x, int y);



        /// <summary>
        /// Set tile data at position ( <paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="data">Input tile data</param>
        /// <exception cref="MD.Map.Core.InvalidTileException"></exception>
        void SetData(IDiggableAccess access, ITileData data);



        Either<InvalidTileException, IDiggableAccess> GetAccessAt(int x, int y);




        /// <summary>
        /// Reduce remaining dig(s) at position ( <paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="reduceVal">Reduce value</param>
        /// <exception cref="MD.Map.Core.InvalidTileException"></exception>
        void Reduce(IDiggableAccess access, int reduceVal);



        /// <summary>
        /// Check whether position ( <paramref name="x"/>, <paramref name="y"/>) has no diggable
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <exception cref="MD.Map.Core.InvalidTileException"></exception>
        Either<InvalidTileException, bool> IsEmptyAt(int x, int y); 


  
        /// <summary>
        /// Logging method for debugging purpose
        /// </summary>    
        void Log();
    }
}