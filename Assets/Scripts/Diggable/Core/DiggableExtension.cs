public static class DiggableExtension
{
    public static bool IsGem(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.COMMON_GEM:
            case DiggableType.UNCOMMON_GEM:
            case DiggableType.RARE_GEM:
            case DiggableType.SUPER_RARE_GEM:
                return true;
            default: return false;
        }
    }

    public static bool IsProjectile(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.NORMAL_BOMB:
                return true;
            default: return false;
        }
    }

    public static DiggableType ToDiggable(this int value)
    {
        switch(value)
        {
            case (int) DiggableType.COMMON_GEM:
                return DiggableType.COMMON_GEM;
            case (int) DiggableType.UNCOMMON_GEM:
                return DiggableType.UNCOMMON_GEM;
            case (int) DiggableType.RARE_GEM:
                return DiggableType.RARE_GEM;
            case (int) DiggableType.NORMAL_BOMB:
                return DiggableType.NORMAL_BOMB;
            default: return DiggableType.EMPTY;
        }
    }
}
