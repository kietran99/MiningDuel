using UnityEngine;
using MD.Character;

namespace MD.VisualEffects
{
    public class HealVFXController : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints = null;

        [SerializeField]
        private ParticleSystem healEffect = null;

        private void Start()
        {
            hitPoints.OnHealSync += Play;
        }

        private void Play() => healEffect.Play();
    }
}