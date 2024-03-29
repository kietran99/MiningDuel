﻿using UnityEngine;
using Functional.Type;
using System;

namespace MD.Diggable.Core
{
    public interface IDiggableGenerator
    {
        Action<Mirror.NetworkConnection, Diggable.Gem.DigProgressData> DigProgressEvent { get; set; }
        Action<Mirror.NetworkConnection, Diggable.Gem.GemObtainData> GemObtainEvent { get; set; }
        Action<Mirror.NetworkConnection, Diggable.Projectile.ProjectileObtainData> ProjectileObtainEvent { get; set; }
        Action<Diggable.DiggableRemoveData> DiggableDestroyEvent { get; set; }
        Action<Diggable.DiggableSpawnData> DiggableSpawnEvent { get; set; }

        SonarTileData[] InitSonarTileData { get; }
        DiggableType RandomDiggableType { get; }
        void DigAt(Mirror.NetworkIdentity digger, int x, int y, int power);
        void BotDigAt(MD.AI.PlayerBot digger, int x, int y, int power);
        DiggableType[] GetDiggableArea(Vector2Int[] positions);
        Either<InvalidTileError, bool> IsProjectileAt(int x, int y); 
        Either<InvalidTileError, bool> IsGemAt(int x, int y);
        Either<InvalidTileError, bool> IsEmptyAt(int x, int y);
    }
}
