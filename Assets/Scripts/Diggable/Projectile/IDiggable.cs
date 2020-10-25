using UnityEngine;

namespace MD.Diggable.Core
{
    public interface IDiggable
    {
        int DigValue { get; }
        Sprite DiggableSprite { get; }
    }
}