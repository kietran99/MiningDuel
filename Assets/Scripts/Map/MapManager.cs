using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager :  MonoBehaviour, IMapManager
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private Transform gemContainer = null;

    [SerializeField]
    private int generateZoneSideLength = 4;

    [SerializeField]
    private int minAmountPerZone = 2;

    [SerializeField]
    private int maxAmountPerZone = 3;

    [SerializeField]
    private GameObject commonGem;
    [SerializeField]
    private GameObject uncommonGem;
    [SerializeField]
    private GameObject rareGem;

    //gem variables
    //gem drop weight large == more chance 
    private int commonDropWeight = 10;
    private int uncommonDropWeight = 5;
    private int rareDropWeight = 2;

    //value to indicate type of gem in map data
    private int uncommonGemValue = 4;
    private int commonGemValue = 1;
    private int rareGemValue = 10;
    #endregion

    #region FIELDS
    private Vector2 mapSize = new Vector2(24,20);
    private float rootX = -12f, rootY = -12f, halfTileSize = .5f;
    private int[,] mapData;

    private float generateDelay = 2f;
    private bool canGenerateNewGem;
    #endregion

    public ScanAreaData GetScanAreaData(Vector2[] posToScan)
    {        
        return new ScanAreaData(GenTileData(posToScan).ToArray());
    }
    
    private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
    {
        foreach (var pos in posToScan)
        {
            yield return new ScanTileData(pos, mapData[(int) pos.x + 12, (int) pos.y + 12]);
        }
    }

    void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        mapData = new int[(int) mapSize.x, (int) mapSize.y];
        GenerateGems();
        canGenerateNewGem = true;
        StartCoroutine(GenerateNewGems());
    }
    private void GenerateGems()
    {
        int areaWidth = (int) mapSize.x / generateZoneSideLength;
        int areaHeight = (int) mapSize.y / generateZoneSideLength;
        int amtPerZone, nGeneratedGems;
        (GameObject prefab, int value) randomGem; 

        for (int y = 0; y < generateZoneSideLength; y++)
        {
            for (int x = 0; x < generateZoneSideLength; x++)
            {
                amtPerZone = Random.Range(minAmountPerZone, maxAmountPerZone + 1);
                nGeneratedGems = 0;
                while (nGeneratedGems < amtPerZone)
                {
                    int randomX = Random.Range(0, areaWidth) + areaWidth* x;
                    int randomY = Random.Range(0, areaHeight) + areaHeight* y;

                    if (mapData[randomX, randomY] != 0) continue;

                    randomGem = GetRandomGem();
                    mapData[randomX, randomY] = randomGem.value;
                    Instantiate(randomGem.prefab, IndexToPosition(new Vector2(randomX, randomY)), 
                        Quaternion.identity, gemContainer);
                    nGeneratedGems++;
                }
            }
        }
        //generate gem-rich areas

    }
    
    (GameObject, int) GetRandomGem()
    {
        int random = Random.Range(1,commonDropWeight + uncommonDropWeight + rareDropWeight + 1);
        if (random <= commonDropWeight)
        {
            return (commonGem, commonGemValue);
        }

        else if (random <= commonDropWeight + uncommonDropWeight)
        {
            return (uncommonGem, uncommonGemValue);
        }
        else
        {
            return (rareGem, rareGemValue);
        }
    }

    //get random empty position's index in mapData return index (-1,-1) if fail
    private Vector2Int GetRandomEmptyIndex()
    {
        int randomX = 0, randomY = 0;
        bool foundLocation  = false;
        //return fail after 10 tries
        int maxTries = 10;
        int timesTried = 0;

        while(!foundLocation)
        {
            randomX = Random.Range(0,(int) mapSize.x);
            randomY = Random.Range(0,(int) mapSize.y);
            if (mapData[randomX, randomY] == 0)
            {
                foundLocation = true;                
            }
            if (timesTried > maxTries)
            {
                Debug.Log("failed to get empty position's index");
                return -Vector2Int.one;
            }
            timesTried++;
        }
        return new Vector2Int(randomX, randomY);
    }
    
    private Vector3 IndexToPosition(Vector2 index)
    {
        return new Vector3(index.x + rootX + halfTileSize, index.y + rootY + halfTileSize, 0f);
    }

    private IEnumerator GenerateNewGems()
    {
        WaitForSeconds waitTime = new WaitForSeconds(generateDelay);
        (GameObject prefab, int value) newGem;
        Vector2Int randomIndex; 
        while(canGenerateNewGem)
        {
            yield return waitTime;
            newGem = GetRandomGem(); //get random gem include rare gem
            randomIndex = GetRandomEmptyIndex();
            //if failed to get empty index
            if (randomIndex == -Vector2Int.one)
            {
                continue;
            }
            //store new gem data to mapData
            mapData[randomIndex.x,randomIndex.y] = newGem.value;
            Instantiate(newGem.prefab, IndexToPosition(randomIndex), Quaternion.identity);
        }
    }

}
