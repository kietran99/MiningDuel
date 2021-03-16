using UnityEngine;

namespace MD.Character
{
    [CreateAssetMenu(fileName="Spawn Position Picker", menuName="Generator/Character/Spawn Position Picker")]
    public class SpawnPositionPicker : ScriptableObject
    {
        [SerializeField]
        private Vector2[] spawnPositions = null;

        private int idx;

        public Vector2 CentreOffset 
        { 
            get => centreOffset;
            set => centreOffset = value;
        }

        private Vector2 centreOffset = new Vector2(0f, 0f);

        private void OnEnable()
        {
            idx = -1;
        }

        public Vector2 NextSpawnPoint
        {
            get
            {
                idx++;
                // Debug.Log("Spawn at: " + (spawnPositions[idx] + CentreOffset));            
                return spawnPositions[idx] + CentreOffset;            
            }
        }
    }
}
