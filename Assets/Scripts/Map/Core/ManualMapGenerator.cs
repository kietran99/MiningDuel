using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.UI;

public class ManualMapGenerator : MonoBehaviour
{
    public int MapWidth => width;
    public int MapHeight => height;
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] string myName = "map";
    // [SerializeField] Tilemap botMap = null;
    // [SerializeField] Tilemap topMap = null;
    // [SerializeField] Tilemap obstacleMap =null;
    // [SerializeField] RuleTile tileNo1= null;
    // [SerializeField] RuleTile tileNo2= null;
    // [SerializeField] RuleTile tileNo3= null;
    // [SerializeField] RuleTile obstacleTile = null;
    [SerializeField] Tilemap[] allLayer = null;
    [SerializeField] RuleTile[] allTile = null;
    [SerializeField] Text noticeText = null;
    int[,] map;
    int[][,] subMap;
    int tileID = 0;
    int selectedMask = 0;
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

    void UpdateNotice()
    {
        if(allTile == null || allLayer == null)
        {
            return;
        }
        if(tileID == -1)
        {
            noticeText.text = "You are now deleting Tile from Layer mask: " + selectedMask.ToString();
        }
        else
            noticeText.text = "You are now using Tile: " + allTile[tileID].name + " on Layer mask: " + selectedMask.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        map = new int[width,height];
        subMap = new int[allLayer.Length][,];
        for(int i = 0; i < allLayer.Length; i++)
        {
            subMap[i] = new int[width,height];
        }
        for(int x = 0; x < width; x++)
            for(int y = 0; y < height; y++)
            {
                subMap[0][x,y] = 1;
            }
        ApplyTiles();
        UpdateNotice();
        Debug.Log("Use Arrow key to move around\nUse MouseScroll to Zoom And LeftClick to draw\nUse Button \"T\" to switch Tile to draw and Button \"L\" to switch tile map to draw on\nFinally, Hit \"S\" to Save");
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
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            tileID = -1;
            UpdateNotice();
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            tileID = 0;
            UpdateNotice();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            tileID = 1;
            UpdateNotice();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            tileID = 2;
            UpdateNotice();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            tileID++;
            if(tileID >= allTile.Length)
            {
                tileID = -1;
            }
            UpdateNotice();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            selectedMask++;
            if(selectedMask >= allLayer.Length)
            {
                selectedMask = 0;
            }
            UpdateNotice();
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
                // if(tileID != -1)
                // {
                //     map[x,y] = tileID;
                // }
                // else
                //     map[x,y] = 0;
                // // ApplyTiles(x,y);
                ApplyTiles(x,y,selectedMask);
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

    void ApplyTiles(int x, int y, int mask)
    {
        if(x < 0 || y < 0 || mask < 0 || allLayer == null || mask >= allLayer.Length) return;
        int weight = map[x,y];
        if(tileID == -1)
        {
            allLayer[mask].SetTile(new Vector3Int(x, y,0), null);
            subMap[mask][x,y] = 0;
        }
        else
            allLayer[mask].SetTile(new Vector3Int(x,y,0),allTile[tileID]);
        subMap[mask][x,y] = tileID + 1;
        int finalIndexVal = allLayer.Length-1;
        for(int i = finalIndexVal; i >= 0; i--)
        {
            if(subMap[i][x,y] == 0)
                continue;
            map[x,y] = subMap[i][x,y] - 1;
            break;
        }
        // map[x,y] = finalIndexVal - 1;
    }
    void ApplyTiles()
    {
        // topMap.ClearAllTiles();
        // botMap.ClearAllTiles();
        // obstacleMap.ClearAllTiles();
        for(int i = 0; i < allLayer.Length; i++)
        {
            allLayer[i].ClearAllTiles();
        }
        if(map!= null)
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {                    
                    // botMap.SetTile(new Vector3Int(x , y , 0), tileNo1);
                    // if(map[x,y] == 1)
                    // {
                    //     topMap.SetTile(new Vector3Int(x , y , 0), tileNo2);
                    // }
                    // else if(map[x,y] == 2)
                    // {
                    //     topMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
                    // }       
                    // else if(map[x,y] == -1)
                    // {
                    //     obstacleMap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
                    // }     
                    allLayer[0].SetTile(new Vector3Int(x,y,0),allTile[0]);      
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
