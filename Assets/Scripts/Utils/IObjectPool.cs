using UnityEngine;

public interface IObjectPool
{
    void Push(GameObject gameObj);
    GameObject Pop();
    void Reset();
}
