using UnityEngine;
using Mirror;

[RequireComponent(typeof(IMapManager))]
public class ProjectileGenerator: MonoBehaviour
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private GameObject normalBombPrefab = null;

    [SerializeField]
    private int maxAmountPerZone = 5;
    #endregion

    #region FIELDS
    private IMapManager mapManager = null; 
    private Vector2Int mapSize = Vector2Int.zero;
    #endregion

    [Server]
    public void StartGenerate(IMapManager mapManager)
    {
        this.mapManager = mapManager;
        mapSize = mapManager.GetMapSize();
        GenerateProjectiles();
    }

    [Server]
    private void GenerateProjectiles()
    {
        Debug.Log("Generate projectiles");
        int HalfMapWidth = mapSize.x/2 + 1;
        int HalfMapHeight = mapSize.y/2 + 1;
        Vector2Int randomIndex = Vector2Int.zero;
        int maxTries = 10;
        for (int x = 0 ; x<=1 ; x++)
        {
            for (int y = 0; y<=1 ; y++)
            {
                int nGeneratedGems = 0;
                while (nGeneratedGems <= maxAmountPerZone)
                {
                    bool success = false;
                    int timesTried = 0;
                    while (!success)
                    {
                        //if fail to spawn after certain tries move to next zone
                        if (timesTried > maxTries)
                        {
                            nGeneratedGems = maxAmountPerZone;
                            break;
                        }
                        randomIndex.x = Random.Range(0 + HalfMapWidth*x, HalfMapWidth + (mapSize.x - HalfMapWidth)*x);
                        randomIndex.y = Random.Range(0 + HalfMapHeight*y, HalfMapHeight + (mapSize.y - HalfMapHeight)*y);
                        success = mapManager.TrySpawnAt(randomIndex, DiggableType.NormalBomb, normalBombPrefab);
                        timesTried++;
                    }
                    nGeneratedGems++;
                }
            }
        }
    }
}