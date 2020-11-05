using EventSystems;
using MD.Diggable;
using MD.Diggable.Gem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

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
public class MapManager : NetworkBehaviour, IMapManager
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
    private int rootX = -12, rootY = -12;
    private float halfTileSize = .5f;
    private SyncList<int> mapData = new SyncList<int>();

    private int ToMapIndex(Vector2Int idx)
    {   
        if (idx.x >= mapSize.x || idx.x < 0 || 
            idx.y >= mapSize.y || idx.y < 0)
        {
            return -1;
        }
        int index = idx.x*mapSize.y + idx.y;
        Debug.Log(mapData.Count);
        if (index < 0 || index >= mapData.Count)
        {
            // Debug.Log("failed at pos x:" +idx.x + " y:" + idx.y);
            return -1;
        }
        return index;
    }

    private bool canGenerateNewGem;
    #endregion

    public ScanAreaData GetScanAreaData(Vector2[] posToScan) => new ScanAreaData(GenTileData(posToScan).ToArray());
    
    private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
    {
        int index;
        foreach (var pos in posToScan)
        {
            index = ToMapIndex(new Vector2Int((int)pos.x - rootX,(int)pos.y-rootY));
            if (index != -1)
            {
                yield return new ScanTileData(pos, mapData[index]);
            }
            else
            {
                yield return new ScanTileData(pos,0);
            }
        }
    }

    // [Server]
    // public void RegisterMapManager()
    // {
    //     Debug.Log("Calling");
    //     RpcRegisterMapManager();
    // }

    // [ClientRpc]
    // void RpcRegisterMapManager()
    // {
    //     Debug.Log("registering mapmanager");
    //     ServiceLocator.Register<IMapManager>(GetComponent<IMapManager>());
    //     IMapManager imap;
    //     ServiceLocator.Resolve<IMapManager>(out imap);
    //     Debug.Log(imap);
    // }

    // void Start()
    // {
    //     EventManager.Instance.StartListening<GemDigSuccessData>(RemoveGemFromMapData);
    // }

    // void OnDestroy()
    // {
    //     EventManager.Instance.StopListening<GemDigSuccessData>(RemoveGemFromMapData);
    // }   

    [Server]
    private void RemoveGemFromMapData(GemDigSuccessData gemDigSuccessData)
    {
        Vector2Int index2D = Vector2Int.zero;
        index2D.x = Mathf.FloorToInt(gemDigSuccessData.posX) - (int)rootX;
        index2D.y = Mathf.FloorToInt(gemDigSuccessData.posY) - (int)rootY;
        int index = ToMapIndex(index2D);
        if (index != -1)
        {
            mapData[index] = 0;
        }
    }

    [Server]
    public void GenerateMap()
    {
        Debug.Log(mapSize.x*mapSize.y);
        for (int i=0; i<(int)mapSize.x*mapSize.y;i++)
        {
            mapData.Add(0);
        }
        Debug.Log(mapData);
        GenerateGems();
        canGenerateNewGem = true;
        // StartCoroutine(GenerateNewGems());
    }

    [Server]
    private void GenerateGems()
    {
        int areaWidth = mapSize.x / generateZoneSideLength;
        int areaHeight = mapSize.y / generateZoneSideLength;
        int amtPerZone, nGeneratedGems, index;
        (GameObject prefab, int value) randomGem; 
        Vector2Int randomPos = Vector2Int.zero;
        for (int y = 0; y < generateZoneSideLength; y++)
        {
            for (int x = 0; x < generateZoneSideLength; x++)
            {
                amtPerZone = Random.Range(minAmountPerZone, maxAmountPerZone + 1);
                nGeneratedGems = 0;
                while (nGeneratedGems < amtPerZone)
                {
                    randomPos.x= Random.Range(0, areaWidth) + areaWidth* x;
                    randomPos.y = Random.Range(0, areaHeight) + areaHeight* y;
                    index = ToMapIndex(randomPos);
                    if (index == -1 || mapData[index] != 0) continue;

                    randomGem = GetRandomGem();
                    mapData[index] = randomGem.value;
                    var Gem = Instantiate(randomGem.prefab, IndexToPosition(randomPos), 
                        Quaternion.identity, gemContainer);
                    NetworkServer.Spawn(Gem);
                    nGeneratedGems++;
                }
            }
        }
        //generate gem-rich areas
    }
    
    [Server]
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
    [Server]
    private Vector2Int GetRandomEmptyIndex()
    {
        Vector2Int randomPos = Vector2Int.zero;
        bool foundLocation  = false;
        
        int maxTries = 10;
        int timesTried = 0;
        int index;

        while(!foundLocation)
        {
            randomPos.x = Random.Range(0, mapSize.x);
            randomPos.y = Random.Range(0, mapSize.y);
            index = ToMapIndex(randomPos);
            if (index != -1 && mapData[index] == 0)
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
        return randomPos;
    }
    
    private Vector3 IndexToPosition(Vector2 index)
    {
        return new Vector3(index.x + rootX + halfTileSize, index.y + rootY + halfTileSize, 0f);
    }

    [Server]
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
            mapData[ToMapIndex(randomIndex)] = newGem.value;
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

    [Server]
    public bool TrySpawnDiggableAtIndex(Vector2Int idx, DiggableType diggable, GameObject prefab)
    {
        // if ( mapData[idx.x,idx.y] != (int) DiggableType.Empty)
        // { 
        //     return false;
        // }
        // mapData[idx.x, idx.y] = (int) diggable;
        // Instantiate(prefab, IndexToPosition(idx), Quaternion.identity, gemContainer);
        return true;
    }   
}
