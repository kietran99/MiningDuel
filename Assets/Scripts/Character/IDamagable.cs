using Mirror;
namespace MD.Character
{
    public interface IDamagable
    {
        void TakeDamage(NetworkIdentity source,int dmg);
    }
}
