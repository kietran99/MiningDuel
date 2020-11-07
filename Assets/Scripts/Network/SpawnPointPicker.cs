using UnityEngine;

public class SpawnPointPicker : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPositions = new Transform[4];

    private int idx;

    public void Reset()
    {
        idx = -1;
    }

    public Transform NextSpawnPoint
    {
        get
        {
            idx++;
            //Debug.Log("Spawn at: " + spawnPositions[idx].position);            
            return spawnPositions[idx];            
        }
    }
}
