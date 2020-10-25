using System;
using UnityEngine;

public interface IObjectPool
{
    void Push(GameObject gameObj);
    GameObject Pop();
    void Reset();
    (GameObject item, int idx) LookUp(Predicate<GameObject> condition);
}
