namespace MD.Diggable.Projectile
{
    public interface IExplodable
    {
        void HandleExplosion(uint throwerID, float gemDropPercentage, int bombType);
    }
}