using UnityEngine;

public static class NumberExtension
{
    public static float Round(this float floatNum)
    {
        bool isPositive = floatNum > 0f;
        float delta = floatNum - Mathf.Floor(floatNum);
        bool shouldRoundUp = isPositive ? delta > .5f : delta < .5f;
        return shouldRoundUp ? Mathf.Ceil(floatNum) : Mathf.Floor(floatNum);
    }

    public static float DeltaInt(this float num_1, float num_2)
    {
        return Mathf.Abs(Mathf.Floor(num_1) - Mathf.Floor(num_2));
    }
}
