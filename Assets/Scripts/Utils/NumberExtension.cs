using UnityEngine;

public static class NumberExtension
{
    public static int Round(this float floatNum)
    {
        bool isPositive = floatNum > 0f;
        float delta = floatNum - Mathf.Floor(floatNum);
        return delta > .45f ? Mathf.CeilToInt(floatNum) : Mathf.FloorToInt(floatNum);
    }

    public static float DeltaInt(this float num_1, float num_2)
    {
        return Mathf.Abs(Mathf.Floor(num_1) - Mathf.Floor(num_2));
    }

    public static bool IsEqual(this float num_1, float num_2)
    {
        return Mathf.Abs(num_1 - num_2) <= Mathf.Epsilon;
    }
}
