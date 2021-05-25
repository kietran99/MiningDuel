using System;
using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class SimWardenRoot : WardenRoot
    {
        [SerializeField]
        private Transform[] players = null;

        [SerializeField]
        private UnityEngine.Tilemaps.Tilemap map = null;

        protected override Transform[] Players => players;

        protected override (Vector2, Vector2) MapBounds => 
            (map.localBounds.max - new Vector3(.5f, .5f, 0f), map.localBounds.min + new Vector3(.5f, .5f, 0f));
    }
}
