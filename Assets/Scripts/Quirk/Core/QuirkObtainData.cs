using UnityEngine;

namespace MD.Quirk
{
    public class QuirkObtainData : EventSystems.IEventData
    {
        public Sprite quirkSprite;
        public string description;

        public QuirkObtainData(Sprite quirkSprite, string description)
        {
            this.quirkSprite = quirkSprite;
            this.description = description;
        }
    }
}