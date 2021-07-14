using UnityEngine;

namespace MD.AI.TheWarden
{
    public interface IWardenParticleController
    {   
        void PlayChaseEffect(Vector2 targetDir);
        void HideChaseEffect();
        void PlayWanderEffect(Vector2 targetDir);
        void HideWanderEffect();
        void PlayAttackEffect();
    }
}
