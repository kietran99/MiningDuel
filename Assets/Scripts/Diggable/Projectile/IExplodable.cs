namespace MD.Diggable.Projectile
{
    public interface IExplodable
    {
        void HandleExplosion(UnityEngine.Transform throwerTransform, uint throwerID, float gemDropPercentage);
        void HandleTrapExplode(float slowDownTime);
    }
}