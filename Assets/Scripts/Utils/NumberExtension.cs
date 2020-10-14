using UnityEngine;

public static class NumberExtension
{
    public static float Round(this float floatNum)
    {        
        return floatNum - .5f < Mathf.Epsilon ? Mathf.Floor(floatNum) : Mathf.Ceil(floatNum);
    }
}
