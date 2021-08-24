using UnityEngine;

namespace MD.Character
{
    [CreateAssetMenu(fileName="Player Color Picker", menuName="Generator/General/Player Color Picker")]
    public class PlayerColorPicker : ScriptableObject
    {
        [SerializeField]
        private Color[] availableColors = null;

        [SerializeField]
        private Sprite blankCrownSprite = null;

        [SerializeField]
        private Sprite[] crownSprites = null;

        private int curIdx;

        private void OnEnable()
        {
           Reset();
        }

        private void Reset() => curIdx = -1;

        public int NextIndex 
        { 
            get
            {
                curIdx = curIdx == (availableColors.Length - 1) ?  0 : (curIdx + 1);
                return curIdx;
            }
        }

        public Color GetColor(int idx) => availableColors[idx];

        public Sprite GetCrownSprite(Color playerColor)
        {
            var res = availableColors.LookUp(color => color == playerColor);
            return res.idx == Constants.INVALID ? blankCrownSprite : crownSprites[res.idx];
        }
    }
}
