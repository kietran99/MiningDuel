using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, IObjectPool
{
    [SerializeField]
    private bool extendable = false;

    [SerializeField]
    private int capacity = 1;

    [SerializeField]
    private GameObject objectToPool = null;

    private string objectName = null;

    private Transform parent = null;

    private Queue<GameObject> pooledObjects, freeObjects;

    void Awake()
    {
        objectName = objectToPool.name;
        parent = transform;
        pooledObjects = new Queue<GameObject>(capacity);
        freeObjects = new Queue<GameObject>(capacity);
        InitObjects(capacity);
    }

    private void InitObjects(int capacity)
    {
        if (capacity > 1) InitObjects(--capacity);

        GameObject instance = Instantiate(objectToPool, parent);
        instance.SetActive(false);
        pooledObjects.Enqueue(instance);        
    }

    public GameObject Pop()
    {
        if (pooledObjects.Count == 0)
        {
            if (extendable)
            {
                GameObject newObj = Instantiate(objectToPool, parent);
                freeObjects.Enqueue(newObj);
                newObj.SetActive(true);
                return newObj;
            }

            return null;
        } 

        GameObject obj = pooledObjects.Dequeue();
        freeObjects.Enqueue(obj);
        obj.SetActive(true);
        return obj;
    }
    
    //if use multiple pools in one script, object to pool's name need to be a distinct value
    //push if is a obj of pool
    public void Push(GameObject gameObj)
    {
        if (gameObj.name == string.Format("{0}(Clone)", objectName))
        {
            gameObj.SetActive(false);
            pooledObjects.Enqueue(gameObj);
        }
    }

    public void Reset()
    {
        if (freeObjects.Count == 0) return;

        var obj = freeObjects.Dequeue();
        obj.SetActive(false);
        pooledObjects.Enqueue(obj);
        Reset();
    }
    
    public (GameObject item, int idx) LookUp(Predicate<GameObject> condition) => freeObjects.ToArray().LookUp(condition);

// #if UNITY_EDITOR
//     private List<GameObject> testActiveObjs = new List<GameObject>();

//     void Update() 
//     { 
//         if (Input.GetKeyDown(KeyCode.M)) 
//         {
//             testActiveObjs.Add(Pop()); 
//         }

//         else if (Input.GetKeyDown(KeyCode.P))
//         {
//             if (testActiveObjs.Count <= 0) return;

//             var randObj = testActiveObjs[UnityEngine.Random.Range(0, testActiveObjs.Count)];
//             Push(randObj);
//             testActiveObjs.Remove(randObj);
//         }

//         else if (Input.GetKeyDown(KeyCode.R)) 
//         {
//             Reset(); 
//             testActiveObjs.Clear();
//         }
//     }
// #endif
}
