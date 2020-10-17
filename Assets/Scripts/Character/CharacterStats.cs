using UnityEngine;

namespace MD.Character
{
    [CreateAssetMenu(fileName = "Character Stats", menuName = "Generator/Character/Stats")]
    public class CharacterStats : ScriptableObject
    {
        [SerializeField]
        private Sprite characterSprite = null;

        [SerializeField]
        private string characterName;

        [SerializeField]
        private int power = 2;

        [SerializeField]
        private int speed = 2;

        [SerializeField]
        [TextArea]
        private string description;

        public Sprite CharacterSprite { get => characterSprite; }
        
        public string CharacterName { get => characterName; }

        public int Power { get => power; }

        public int Speed { get => speed; }
    }
}