using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class TestGrid
{
    public int height,width;
    public float cellSize;
    
    int[,] gridArray;

    public TestGrid(int height, int width, float cellSize)
    {
        this.height = height;
        this.width = width;
        this.cellSize = cellSize;

        gridArray = new int[width,height];
        DrawGrid();
    }

    public void DrawGrid()
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y= 0; y< height; ++y)
            {
                Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x, y+1), Color.white, 3600f);
                Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x + 1, y), Color.white, 3600f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width, height), Color.white, 3600f);
        Debug.DrawLine(GetWorldPosition(width, 0),GetWorldPosition(width, height), Color.white, 3600f);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y)*cellSize;
    }

    public Vector2Int GetIndex(Vector3 WorldPosition)
    {
        Vector2Int index = Vector2Int.zero;
        index.x = Mathf.FloorToInt(WorldPosition.x/cellSize);
        index.y = Mathf.FloorToInt(WorldPosition.y/cellSize);
        return index;
    }

    public void SetValue(int x, int y,int value)
    {
        Debug.Log("set value "+ value + " at " + x + " " + y);
        try
        {
            gridArray[x, y] = value;
        }
        catch
        {
            Debug.Log("CantSetValue: out of bound");
        }
    }

    public int GetValue(int x, int y)
    {
        if (IsIndexValid(x,y)) return gridArray[x,y];
        return -1;
    }

    public bool IsIndexValid(int x, int y)
    {
        if (x >= 0 && x < gridArray.GetLength(0) && y >=0 && y < gridArray.GetLength(1) ) return true;
        return false;
    }

    public bool IsWalkable(Vector2Int from, Vector2Int to)
    {
        if (IsIndexValid(to.x,to.y) && IsIndexValid(from.x,from.y))
        {
            //if obstacle
            if (gridArray[to.x,to.y] == -1) return false;
            
            //Check if move diagonal
            if ((from.x != to.x) && (from.y != to.y))
            {
                if (gridArray[from.x,to.y] == -1 && gridArray[to.x, from.y] == -1) return false;
            }
        }
        else
        {
            Debug.Log("index out of bound");
            return false;
        }
        return true;
    }

    public void SetValue(Vector3 WorldPosition, int value)
    {
        Vector2Int index = GetIndex(WorldPosition);
        SetValue(index.x, index.y, value);
    }
}
public class DrawGrid : MonoBehaviour
{
    // Start is called before the first frame update
    private TestGrid grid;
    private AStar aStar;
    [SerializeField]
    private GameObject obstacle;
    [SerializeField]
    private GameObject road;
    [SerializeField]
    private GameObject goal;


    private Vector2Int characterIndex = Vector2Int.zero;
    void Start()
    {
        grid = new TestGrid(20,20,10f);
        aStar = new AStar(20,20, grid.IsWalkable);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("clicked at"  + Input.mousePosition);
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;
            
            Debug.Log("clicked at" + pos);
            Vector2Int index = grid.GetIndex(pos);
            if (grid.GetValue(index.x,index.y) == -1) return;
            Vector3 worldPos = grid.GetWorldPosition(index.x,index.y) + Vector3.one*grid.cellSize/2f;
            worldPos.z = 0f;
            GameObject obj = Instantiate(obstacle, worldPos, Quaternion.identity);

            grid.SetValue(pos,-1);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("clicked at"  + Input.mousePosition);
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;
            Vector2Int goalIndex = grid.GetIndex(pos);
            Debug.Log("find path goal " + goalIndex );

            List<PathFinding.Node> path = aStar.FindPath(characterIndex, goalIndex);
            if (path != null)
            {
                GameObject[] lastPath = GameObject.FindGameObjectsWithTag("CheckPoint");
                foreach (GameObject obj in lastPath) Destroy(obj);
                //visualize start position
                Instantiate(road, grid.GetWorldPosition(characterIndex.x,characterIndex.y) + Vector3.one*grid.cellSize/2f, Quaternion.identity);

                for (int i=0; i< path.Count ; ++i)
                {
                    Vector3 worldPos = grid.GetWorldPosition(path[i].index.x,path[i].index.y) + Vector3.one*grid.cellSize/2f;
                    worldPos.z = 0f;
                    if (i == path.Count - 1 ) Instantiate(goal, worldPos, Quaternion.identity);
                    Instantiate(road, worldPos, Quaternion.identity);
                }
                characterIndex = goalIndex;
            }
        }
    }
}
