namespace MD.Character
{
    public interface IDamagable
    {
        void TakeDamage(Mirror.NetworkIdentity source, int dmg, bool isCritical);
    }
}
