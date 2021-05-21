using MD.Character;
using Mirror;

public class TestDamagable : NetworkBehaviour, IDamagable
{
    public void TakeDamage(NetworkIdentity source, int dmg, bool isCritical)
    {
        // EventSystems.EventManager.Instance.TriggerEvent(new DamageTakenData(netId, new UnityEngine.Vector2(0f, 0f)));
        // EventSystems.EventManager.Instance.TriggerEvent(new HPChangeData(100, 80, 100));
        EventSystems.EventManager.Instance.TriggerEvent(new DamageGivenData(transform.position, dmg, isCritical));
    }
}
