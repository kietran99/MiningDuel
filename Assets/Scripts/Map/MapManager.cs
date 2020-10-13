using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager :  MonoBehaviour, IMapManager
{
    // map's size = (width, Height) 
    private Vector2 mapSize = new Vector2(24,20);
    //bottom left position of the map on grid
    float rootX = -12f;
    float rootY = -12f;
    //half tile size
    private float halfTileSize = .5f;
    //array to store info of all diggable items in the map
    private int[,] mapData;

    //time wait between generating new gems
    private float generateDelay = 10f;
    private bool canGenerateNewGem;

    //Gem prefabs
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
  

    //return all scanTileData of a desired area
    public ScanAreaData GetScanAreaData(Vector2[] tilesToScan)
    {
        ScanAreaData result;
        ScanTileData[] tileData = new ScanTileData[tilesToScan.Length];
        ScanTileData tile;
        int idx = 0;
        foreach (Vector2 tilePos in tilesToScan)
        {
            //get tile corresponding to the tile pos 
            tile = new ScanTileData(tilePos, mapData[(int) tilePos.x,(int) tilePos.y]);
            tileData[idx] = tile;
            idx++;
        }
        result = new ScanAreaData(tileData);
        return result;
    }

    // generate map at the start of the match

    void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        mapData = new int[(int) mapSize.x,(int) mapSize.y];
        GenerateGems();
        canGenerateNewGem = true;
        StartCoroutine(GenerateNewGems());
    }
    // generate base gem of the map
    private void GenerateGems()
    {
        //generate gems spreading equally on the map
        //devide map into n*n areas with equal maximum number of gems
        //map size needs to be a multiple of n
        int n = 4;
        int areaWidth = (int) mapSize.x/n;
        int areaHeight = (int) mapSize.y/n;
        //generate up to 3 gems in every area
        int gemPerArea;
        int numberOfGeneratedGems;
        (GameObject prefab, int value) randomGem; //stores random gem prefab and its value
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x< n; x++)
            {
                gemPerArea = Random.Range(2,4);
                numberOfGeneratedGems = 0;
                while (numberOfGeneratedGems < gemPerArea)
                {
                    int randomX = Random.Range(0, areaWidth) + areaWidth*x;
                    int randomY = Random.Range(0, areaHeight) + areaHeight*y;
                    //if random postion havent has gem yet
                    if (mapData[randomX,randomY] == 0)
                    {
                        randomGem= GetRandomGem();
                        mapData[randomX,randomY] = randomGem.value;
                        Instantiate(randomGem.prefab,IndexToPosition(new Vector2(randomX, randomY)),Quaternion.identity);
                        numberOfGeneratedGems++;
                    }
                }
            }
        }
        //generate gem-rich areas

    }
    //get random gem prefab and its value
    //chance to get gem base on its drop weight
    (GameObject,int) GetRandomGem()
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
        int randomX = 0,randomY = 0;
        bool foundLocation  = false;
        //return fail after 10 tries
        int maxtries = 10;
        int timestried = 0;
        while(!foundLocation)
        {
            randomX = Random.Range(0,(int) mapSize.x);
            randomY = Random.Range(0,(int) mapSize.y);
            if (mapData[randomX, randomY] == 0)
            {
                foundLocation = true;                
            }
            if (timestried > maxtries)
            {
                Debug.Log("failed to get empty position's index");
                return -Vector2Int.one;
            }
            timestried++;
        }
        return new Vector2Int(randomX, randomY);
    }
    
    //transform index in mapdata to world position
    private Vector3 IndexToPosition(Vector2 index)
    {
        return new Vector3(index.x + rootX + halfTileSize, index.y + rootY + halfTileSize, 0f);
    }
    // generate new gem every x seconds
    private IEnumerator GenerateNewGems()
    {
        WaitForSeconds waitTime = new WaitForSeconds(generateDelay);
        (GameObject prefab, int value) newGem;
        Vector2Int randomIndex; //index of gem in mapdata
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
