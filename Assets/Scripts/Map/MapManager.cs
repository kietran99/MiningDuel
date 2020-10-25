using EventSystems;
using MD.Diggable;
using MD.Diggable.Gem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapDataTypeExtensions
{
    public static bool IsGem(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.CommonGem:
            case DiggableType.UncommonGem:
            case DiggableType.RareGem:
                return true;
            default: return false;
        }
    }

    public static bool IsBomb(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.NormalBomb:
                return true;
            default: return false;
        }
    }
}
public class MapManager : MonoBehaviour, IMapManager
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
    private float generateDelay = 2f;

    [SerializeField]
    private GameObject commonGem = null;
    [SerializeField]
    private GameObject uncommonGem = null;
    [SerializeField]
    private GameObject rareGem = null;
    #endregion

    #region FIELDS
    private int commonDropWeight = 10;
    private int uncommonDropWeight = 5;
    private int rareDropWeight = 2;

    // private int uncommonGemValue = 4;
    // private int commonGemValue = 1;
    // private int rareGemValue = 10;

    private Vector2Int mapSize = new Vector2Int(24,20);
    private float rootX = -12f, rootY = -12f, halfTileSize = .5f;
    private int[,] mapData;

    private bool canGenerateNewGem;
    #endregion

    public ScanAreaData GetScanAreaData(Vector2[] posToScan) => new ScanAreaData(GenTileData(posToScan).ToArray());
    
    private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
    {
        foreach (var pos in posToScan)
        {
            if (pos.x - (int)rootX < mapData.GetLength(0) && 
                pos.x - (int)rootX >= 0 && 
                pos.y - (int)rootY < mapData.GetLength(1) && 
                pos.y - (int)rootY >= 0)
            {
                yield return new ScanTileData(pos, mapData[(int) pos.x - (int)rootX, (int) pos.y - (int)rootY]);
            }
            else
            {
                yield return new ScanTileData(pos,0);
            }
        }
    }

    void Awake()
    {
        GenerateMap();
    }

    void Start()
    {
        EventManager.Instance.StartListening<GemDigSuccessData>(RemoveGemFromMapData);
    }

    void OnDestroy()
    {
        EventManager.Instance.StopListening<GemDigSuccessData>(RemoveGemFromMapData);
    }   

    private void RemoveGemFromMapData(GemDigSuccessData gemDigSuccessData)
    {
        int indexX = Mathf.FloorToInt(gemDigSuccessData.posX) - (int)rootX;
        int indexY = Mathf.FloorToInt(gemDigSuccessData.posY) - (int)rootY;
        if (indexX >= 0 && indexX < mapData.GetLength(0) && indexY >= 0 && indexY < mapData.GetLength(1))
        {
            mapData[indexX, indexY] = 0;
        }
    }

    public void GenerateMap()
    {
        mapData = new int[mapSize.x, mapSize.y];
        GenerateGems();
        canGenerateNewGem = true;
        StartCoroutine(GenerateNewGems());
    }

    private void GenerateGems()
    {
        int areaWidth = mapSize.x / generateZoneSideLength;
        int areaHeight = mapSize.y / generateZoneSideLength;
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
    
    private (GameObject, int) GetRandomGem()
    {
        int random = Random.Range(1, commonDropWeight + uncommonDropWeight + rareDropWeight + 1);
        if (random <= commonDropWeight)
        {
            return (commonGem, (int) DiggableType.CommonGem);
        }

        if (random <= commonDropWeight + uncommonDropWeight)
        {
            return (uncommonGem, (int) DiggableType.UncommonGem);
        }
        
        return (rareGem, (int) DiggableType.RareGem);
    }

    private Vector2Int GetRandomEmptyIndex()
    {
        int randomX = 0, randomY = 0;
        bool foundLocation  = false;
        
        int maxTries = 10;
        int timesTried = 0;

        while(!foundLocation)
        {
            randomX = Random.Range(0, mapSize.x);
            randomY = Random.Range(0, mapSize.y);

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
        Vector3 worldPostion;

        while(canGenerateNewGem)
        {
            yield return waitTime;
            randomIndex = GetRandomEmptyIndex();
            if (randomIndex == -Vector2Int.one)
            {
                continue;
            }
            newGem = GetRandomGem(); 
            worldPostion = IndexToPosition(randomIndex);
            mapData[randomIndex.x, randomIndex.y] = newGem.value;
            Instantiate(newGem.prefab, worldPostion, Quaternion.identity, gemContainer);
            EventSystems.EventManager.Instance.TriggerEvent(
                new GemSpawnData(worldPostion.x - MapConstants.SPRITE_OFFSET.x, 
                worldPostion.y - MapConstants.SPRITE_OFFSET.y, (DiggableType)newGem.value));
        }
    }

    public Vector2Int GetMapSize() => mapSize;

    public Vector2Int PositionToIndex(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x),Mathf.FloorToInt(position.y));
    }

    public bool TrySpawnDiggableAtIndex(Vector2Int idx, DiggableType diggable, GameObject prefab)
    {
        if ( mapData[idx.x,idx.y] != (int) DiggableType.Empty)
        { 
            return false;
        }
        mapData[idx.x, idx.y] = (int) diggable;
        Instantiate(prefab, IndexToPosition(idx), Quaternion.identity, gemContainer);
        return true;
    }   
}
