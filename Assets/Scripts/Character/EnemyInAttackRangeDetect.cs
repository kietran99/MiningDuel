using UnityEngine;

namespace MD.Character
{
    public class EnemyInAttackRangeDetect : MonoBehaviour
    {
        [SerializeField]
        private Transform pickaxe = null;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damagable = other.GetComponent<IDamagable>();

            if (damagable == null)
            {
                return;
            }

            pickaxe.transform.Rotate(0f, 0f, Vector2.SignedAngle(-pickaxe.transform.up, other.transform.position - pickaxe.transform.position));
        }
    }
}
