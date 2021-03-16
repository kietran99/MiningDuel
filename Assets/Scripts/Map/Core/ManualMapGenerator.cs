using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ManualMapGenerator : MonoBehaviour
{
    public int MapWidth => width;
    public int MapHeight => height;
    [SerializeField] int width = 0;
    [SerializeField] int height = 0;
    [SerializeField] Tilemap botMap = null;
    [SerializeField] Tilemap topMap = null;
    [SerializeField] Tilemap obstacleMap =null;
    [SerializeField] Tile tileNo1= null;
    [SerializeField] Tile tileNo2= null;
    [SerializeField] Tile tileNo3= null;
    int[,] map;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    }
                    Vector3 pos = new Vector3( x+.5f, y +.5f,0);
                    Gizmos.DrawCube(pos,Vector3.one); 
                }
            }
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
                        obstacleMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
                    }           
                }
            }
        }
    }
    void SaveArrayData()
    {

    }
}
