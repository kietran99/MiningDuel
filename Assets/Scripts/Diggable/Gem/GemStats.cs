using MD.Diggable.Core;
using UnityEngine;

namespace MD.Diggable.Gem
{
    [CreateAssetMenu(fileName = "Gem Stats", menuName = "Generator/Diggable/Gem/Stats")]
    public class GemStats : ScriptableObject, IDiggable
    {
        [SerializeField]
        private Sprite gemSprite = null;

        [SerializeField]
        private Sprite sonarSprite = null;

        [SerializeField]
        private int value = 1;

        public int DigValue { get => value; }
        public Sprite WorldSprite { get => gemSprite; }
        public Sprite SonarSprite { get => sonarSprite; }
    }
}