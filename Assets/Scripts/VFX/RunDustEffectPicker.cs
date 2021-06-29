using UnityEngine;

namespace MD.VisualEffects
{
    public class RunDustEffectPicker : MonoBehaviour
    {
        [SerializeField]
        private GameObject regularParticleSystem = null;

        [SerializeField]
        private GameObject boostedParticleSystem = null;

        [SerializeField]
        private Mirror.NetworkIdentity player = null;

        private void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.SpeedBoostData>(OnSpeedBoost);
        }

        private void OnSpeedBoost(Character.SpeedBoostData data)
        {
            if (data.userId != player.netId)
            {
                return;
            }

            regularParticleSystem.SetActive(false);
            boostedParticleSystem.SetActive(true);
            Invoke(nameof(ToRegular), data.time);
        }

        private void ToRegular()
        {
            regularParticleSystem.SetActive(true);
            boostedParticleSystem.SetActive(false);
        }
    }
}
