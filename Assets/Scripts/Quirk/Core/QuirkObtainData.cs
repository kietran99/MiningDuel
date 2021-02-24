using UnityEngine;

namespace MD.Quirk
{
    public class QuirkObtainData : EventSystems.IEventData
    {
        public Sprite quirkSprite;

        public QuirkObtainData(Sprite quirkSprite)
        {
            this.quirkSprite = quirkSprite;
        }
    }
}