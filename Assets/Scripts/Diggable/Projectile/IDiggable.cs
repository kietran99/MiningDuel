using UnityEngine;

namespace MD.Diggable.Core
{
    public interface IDiggableStats
    {
        int DigValue { get; }
        Sprite WorldSprite { get; }
        Sprite SonarSprite { get; }
    }
}