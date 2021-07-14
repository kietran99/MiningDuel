using UnityEngine;

namespace MD.AI.TheWarden
{
    public class BaseWardenAttackChargeIndicator : MonoBehaviour, IWardenAttackChargeIndicator
    {
        [SerializeField]
        private GameObject indicator = null;

        public void Show()
        {
            indicator.transform.localScale = new Vector3(1f, 1f, 1f);
            indicator.gameObject.SetActive(true);
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
