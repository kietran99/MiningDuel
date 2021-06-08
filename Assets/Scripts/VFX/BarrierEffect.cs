using UnityEngine;

namespace MD.VisualEffects
{
    public class BarrierEffect : MonoBehaviour
    {
        [SerializeField]
        private Mirror.NetworkIdentity player = null;

        [SerializeField]
        private Animator animator = null;

        private uint playerId;
        private readonly string PLAY = "Play";

        private void Start()
        {
            playerId = player.netId;
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Diggable.Projectile.ActivateLinkedTrapEvent>(
                _ => animator.SetTrigger(PLAY), data => data.activatorId == playerId);
        }
    }
}