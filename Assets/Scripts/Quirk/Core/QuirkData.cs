using UnityEngine;

namespace MD.Quirk
{
    [CreateAssetMenu(fileName="Quirk Data", menuName="Generator/Quirk/Quirk Data")]
    public class QuirkData : ScriptableObject
    {
        [SerializeField]
        private Sprite obtainSprite = null;

        [TextArea]
        [SerializeField]
        private string description = string.Empty;

        [SerializeField]
        private string QuirkName = string.Empty;

        public Sprite ObtainSprite => obtainSprite;

        public string Description => description;

        public string Name => QuirkName;
    }
}