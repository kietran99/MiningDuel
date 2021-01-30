using UnityEngine;
using Functional.Type;
using System;

namespace MD.Map.Core
{
    public interface IDiggableGenerator
    {
        // Events are invoked on every authorative communicator on each client
        Action<Mirror.NetworkConnection, Diggable.Gem.DigProgressData> DigProgressEvent { get; set; }
        Action<Mirror.NetworkConnection, Diggable.Gem.GemObtainData> GemObtainEvent { get; set; }
        Action<Mirror.NetworkConnection, Diggable.Projectile.ProjectileObtainData> ProjectileObtainEvent { get; set; }
        Action<Diggable.DiggableRemoveData> DiggableDestroyEvent { get; set; }

        void SetTile(Vector2Int pos, DiggableType type);
        void Populate(Vector2Int[] tilePositions);
        void DigAt(Mirror.NetworkIdentity digger, int x, int y, int power);
        DiggableType[] GetDiggableArea(Vector2Int[] positions);
    }
}
