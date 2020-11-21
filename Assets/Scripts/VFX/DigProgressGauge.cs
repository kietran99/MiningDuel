using UnityEngine;

namespace MD.VisualEffects
{
    public class DigProgressGauge : MonoBehaviour
    {
        [SerializeField]
        private Transform fillArea = null;
        
        public void Fill(int cur, int max)
        {
            float fraction = (float) cur / (float) max;
            Debug.Log("Fraction to dig successful: " + fraction);
            fillArea.localScale = new Vector3(fraction, fillArea.localScale.y, 1f);
            fillArea.localPosition = new Vector3(-1f + fraction, fillArea.localPosition.y, fillArea.localPosition.z);
        }
    }
}
