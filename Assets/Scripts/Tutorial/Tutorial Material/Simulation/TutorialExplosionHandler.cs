using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialExplosionHandler : MonoBehaviour, Diggable.Projectile.IExplodable
    {
        [SerializeField]
        private GameObject droppingGemPrefab = null;

        [SerializeField]
        private int dropAmount = 10;
 
        [SerializeField]
        private float maxExplosionForce = 250f;
        
        public void HandleExplosion(Transform thrower, uint _, float dropPercentage)
        {
            for (int i = 0; i < dropAmount; i++)
            {
                GameObject droppingGem = Instantiate(droppingGemPrefab, transform.position, Quaternion.identity);
                droppingGem.GetComponent<TutorialDropDriver>().ThrowerTransform = thrower;
                droppingGem.GetComponent<Rigidbody2D>().AddForce(Random.Range(100f, maxExplosionForce) * new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            }
        }

        public void HandleTrapExplode(float slowDownTime){}
    }
}
