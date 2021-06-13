using UnityEngine;

namespace MD.AI.TheWarden
{
    public class SimWardenAttackChargeIndicator : MonoBehaviour, IWardenAttackChargeIndicator
    {
        [SerializeField]
        private GameObject indicator = null;

        public void Show()
        {
            indicator.gameObject.SetActive(true);
            Scale(1f);
        }

        public void Scale(float scale)
        {
            indicator.transform.localScale = new Vector3(scale, scale, 1f);
        }

        public void Hide()
        {
            indicator.gameObject.SetActive(false);
        }
    }
}
