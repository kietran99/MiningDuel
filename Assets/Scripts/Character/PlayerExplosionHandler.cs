using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    [RequireComponent(typeof(Player))]
    public class PlayerExplosionHandler : MonoBehaviour, IExplodable
    {
        #region  SERIALIZE FIELDS
        [SerializeField]
        private GameObject droppingGemPrefab = null;

        [SerializeField]
        private float maxExplosionForce = 250f;
        #endregion

        private ScoreManager scoreManager;
        private uint playerId;

        void Start()
        {
            scoreManager = GetComponent<ScoreManager>();
            playerId = GetComponent<NetworkIdentity>().netId;
        }

        [Server]
        public void HandleExplosion(Transform throwerTransform, uint throwerID, float gemDropPercentage, int projectileType)
        {
            int dropAmount = Mathf.FloorToInt(scoreManager.CurrentMultiplier * scoreManager.CurrentScore * gemDropPercentage * .01f);
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodedData(playerId, dropAmount));

            for (int i = 0; i < dropAmount; i++)
            {
                GameObject droppingGem = Instantiate(droppingGemPrefab, transform.position, Quaternion.identity);
                NetworkServer.Spawn(droppingGem);
                droppingGem.GetComponent<Diggable.Gem.DropObtain>().ThrowerID = throwerID;
                droppingGem.GetComponent<Diggable.Gem.DropDriver>().ThrowerTransform = throwerTransform;
                droppingGem.GetComponent<Rigidbody2D>().AddForce(RandomExplosionForce() * RandomExplosionDirection());
            }
        }

        private float RandomExplosionForce()
        {
            return Random.Range(100f, maxExplosionForce);
        }

        private Vector2 RandomExplosionDirection()
        {
            Vector2 randomDir = Vector2.zero;
            randomDir.x = Random.Range(-1f, 1f);
            randomDir.y = Random.Range(-1f, 1f);
            return randomDir.normalized;
        }
    }
}