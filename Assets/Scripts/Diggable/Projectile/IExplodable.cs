namespace MD.Diggable.Projectile
{
    public interface IExplodable
    {
        void HandleExplosion(float gemDropPercentage, int bombType);
    }
}