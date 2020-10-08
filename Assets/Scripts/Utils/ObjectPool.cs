using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, IObjectPool
{
    [SerializeField]
    private int capacity = 1;

    [SerializeField]
    private GameObject objectToPool = null;

    [SerializeField]
    private GameObject parent = null;

    private Queue<GameObject> pooledObjects, freeObjects;

    void Awake()
    {
        pooledObjects = new Queue<GameObject>(capacity);
        freeObjects = new Queue<GameObject>(capacity);
        InitObjects(capacity);
    }

    private void InitObjects(int capacity)
    {
        if (capacity > 1) InitObjects(--capacity);

        GameObject instance = Instantiate(objectToPool, parent.transform);
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
}
