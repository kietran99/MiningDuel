using UnityEngine;

namespace MD.Character
{
    public class AlwaysGetCounteredEnabler : MonoBehaviour
    {
        [SerializeField]
        private AlwaysGetCounteredZone damageZone = null;

        void Start()
        {
            damageZone.OnGetCountered += DelayedEnable;
        }

        void OnDestroy()
        {
            damageZone.OnGetCountered -= DelayedEnable;
        }

        private void DelayedEnable(Vector2 obj)
        {
            Invoke(nameof(Enable), .5f); // <- .5f is the mobilize time field on Basic Attack Action
        }

        private void Enable() => damageZone.gameObject.SetActive(true);
    }
}
