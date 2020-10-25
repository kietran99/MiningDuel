using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, IObjectPool
{
    [SerializeField]
    private int capacity = 1;

    [SerializeField]
    private GameObject objectToPool = null;

    private Transform parent = null;

    private Queue<GameObject> pooledObjects, freeObjects;

    void Awake()
    {
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
        if (pooledObjects.Count == 0) return null;

        GameObject obj = pooledObjects.Dequeue();
        freeObjects.Enqueue(obj);
        obj.SetActive(true);
        return obj;
    }

    public void Push(GameObject gameObj)
    {
        gameObj.SetActive(false);
        pooledObjects.Enqueue(gameObj);
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
}
