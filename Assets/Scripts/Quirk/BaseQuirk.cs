using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseQuirk : MonoBehaviour
{
    [SerializeField] int expireTime = 5;
    void Expire()
    {
        // do sumting
    }
    void StartTimer()
    {
        Invoke("Expire", expireTime);
        CastEffect();
    }

    void CastEffect()
    {
        // Different for each Quirk
    }
}
