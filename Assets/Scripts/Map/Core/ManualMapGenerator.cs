using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class ManualMapGenerator : MonoBehaviour
{
    public int MapWidth => width;
    public int MapHeight => height;
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] string myName = "map";
    [SerializeField] Tilemap botMap = null;
    [SerializeField] Tilemap topMap = null;
    [SerializeField] Tilemap obstacleMap =null;
    [SerializeField] RuleTile tileNo1= null;
    [SerializeField] RuleTile tileNo2= null;
    [SerializeField] RuleTile tileNo3= null;
    [SerializeField] RuleTile obstacleTile = null;
    int[,] map;
    int tileID = 0;
    // string seed = "";
    public int GetElement(int x, int y)
    {
        if(x < 0 || y < 0)
        {
            Debug.LogError("Invalid Index when getitng element! x= " + x+" y = " + y);
            return -1;
        }
        return map[x,y];
        // return 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        map = new int[width,height];
        ApplyTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            map = new int[width,height];
            ApplyTiles();
        }
        Camera.main.transform.Translate(new Vector3(0,0,Input.GetAxis("Mouse ScrollWheel")*10));
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.Translate(new Vector3(-10*Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.Translate(new Vector3(10*Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.Translate(new Vector3(0,10*Time.deltaTime,0));
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.Translate(new Vector3(0,-10*Time.deltaTime,0));
        }
        switch(tileID)
        {
            case 0:
                Debug.Log("Apllying Tile: tileNo1");
                break;
            case 1:
                Debug.Log("Apllying Tile: tileNo2");
                break;
            case 2:
                Debug.Log("Apllying Tile: tileNo3");
                break;
            case -1:
                Debug.Log("Apllying Tile: obstacleTile");
                break;
        }
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            tileID = -1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            tileID = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            tileID = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            tileID = 2;
        }
        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            int x = (int)mousePos.x;
            int y = (int)mousePos.y;
            if( x>=0 && x < width && y>= 0 && y <height)
            {
                map[x,y] = tileID;
                ApplyTiles(x,y);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveArrayData();
        }
        
    }
    void OnDrawGizmos()
    {
        if(map!= null)
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y< height; y++)
                {
                    switch(map[x,y])
                    {
                        case 0:
                            Gizmos.color = Color.white;
                            break;
                        case 1:
                            Gizmos.color = Color.black;
                            break;
                        case 2:
                            Gizmos.color = Color.blue;
                            break;
                        case -1:
                            Gizmos.color = Color.red;
                            break;
                    }
                    Vector3 pos = new Vector3( x+.5f, y +.5f,0);
                    Gizmos.DrawCube(pos,Vector3.one); 
                }
            }
        }
    }

    void ApplyTiles(int x, int y)
    {
        if(x < 0 || y < 0) return;
        int weight = map[x,y];
        switch(weight)
        {
            case 0:
                topMap.SetTile(new Vector3Int(x, y, 0),null);
                obstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                break;
            case 1:
                obstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                topMap.SetTile(new Vector3Int(x, y, 0), tileNo2);
                break;
            case 2:
                obstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                topMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
                break;
            case -1:
                topMap.SetTile(new Vector3Int(x, y, 0), null);
                obstacleMap.SetTile(new Vector3Int(x, y,0), obstacleTile);
                break;
        }

    }
    void ApplyTiles()
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        obstacleMap.ClearAllTiles();
        if(map!= null)
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {                    
                    botMap.SetTile(new Vector3Int(x , y , 0), tileNo1);
                    if(map[x,y] == 1)
                    {
                        topMap.SetTile(new Vector3Int(x , y , 0), tileNo2);
                    }
                    else if(map[x,y] == 2)
                    {
                        topMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
                    }       
                    else if(map[x,y] == -1)
                    {
                        obstacleMap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
                    }           
                }
            }
        }
    }
    void SaveArrayData()
    {
        #if UNITY_EDITOR
        string saveName = myName;
        string fileName = "";
        SaveMapData.SaveArray(this,saveName,out fileName);
        EditorUtility.DisplayDialog("Map Data saved","The file was saved under Streaming Assests with name: " + fileName,"Continue");
        #endif
    }
}
