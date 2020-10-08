public static class StringFormatExtension
{
    public static string FormatNum(this int num)
    {
        return (num < 10 ? "0" : "") + num;
    }

    public static int IntFastParse(this string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }
}
