using UnityEngine;

namespace MD.Character
{
    [CreateAssetMenu(fileName="Spawn Position Picker", menuName="Generator/Character/Spawn Position Picker")]
    public class SpawnPositionPicker : ScriptableObject
    {
        [SerializeField]
        private Vector2[] spawnPositions = null;

        public Vector2[] SpawnPositions => spawnPositions;
    }

    public struct SpawnPositionsData
    {
        private int idx;
        private Vector2[] spawnPositions;

        public SpawnPositionsData(Vector2[] spawnPositions)
        {
            this.idx = -1;
            this.spawnPositions = spawnPositions;
        }

        public Vector2 NextSpawnPoint
        {
            get
            {
                idx++;
                // Debug.Log("Spawn at: " + (spawnPositions[idx] + CentreOffset));            
                return spawnPositions[idx];            
            }
        }
    }
}
