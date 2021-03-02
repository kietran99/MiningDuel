using UnityEngine;

namespace MD.Character
{
    [CreateAssetMenu(fileName="Player Color Picker", menuName="Generator/Player Color Picker")]
    public class PlayerColorPicker : ScriptableObject
    {
        [SerializeField]
        private Color[] availableColors = null;

        private int curIdx;

        private void OnEnable()
        {
           Reset();
        }

        private void Reset() => curIdx = -1;

        public int NextIndex => ++curIdx;

        public Color GetColor(int idx) => availableColors[idx];
    }
}
