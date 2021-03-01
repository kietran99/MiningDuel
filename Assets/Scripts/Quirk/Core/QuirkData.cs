using UnityEngine;

namespace MD.Quirk
{
    [CreateAssetMenu(fileName="Quirk Data", menuName="Generator/Quirk Data")]
    public class QuirkData : ScriptableObject
    {
        [SerializeField]
        private Sprite obtainSprite = null;

        [TextArea]
        [SerializeField]
        private string description = string.Empty;

        public Sprite ObtainSprite => obtainSprite;

        public string Description => description;
    }
}