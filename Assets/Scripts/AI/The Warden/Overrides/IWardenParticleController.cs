namespace MD.AI.TheWarden
{
    public interface IWardenParticleController
    {   
        void PlayChaseEffect(UnityEngine.Vector2 targetDir);
        void HideChaseEffect();
        void PlayAttackEffect();
    }
}
