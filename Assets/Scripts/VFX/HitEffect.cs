using UnityEngine;

namespace MD.VisualEffects
{
    public class HitEffect : MonoBehaviour
    {
        [SerializeField]
        private float _effectDuration = .5f;

        [SerializeField]
        private ParticleSystem _particleSystem = null;

        private System.Action<HitEffect> OnEffectEnd;

        public void Play(System.Action<HitEffect> OnEffectEndCallback)
        {
            OnEffectEnd = OnEffectEndCallback;
            _particleSystem.Play();
            Invoke(nameof(RaiseEffectEndEvent), _effectDuration);
        }

        public void RaiseEffectEndEvent()
        {
            _particleSystem.Stop();
            OnEffectEnd?.Invoke(this);
        }
    }
}
