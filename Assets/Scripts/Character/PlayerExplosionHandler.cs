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

        public void HandleExplosion(float gemDropPercentage, int bombType)
        {
            int dropAmount = Mathf.FloorToInt(scoreManager.CurrentScore * gemDropPercentage * .01f);
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodedData(playerId, dropAmount));

            for (int i = 0; i < dropAmount; i++)
            {
                GameObject droppingGem = Instantiate(droppingGemPrefab, transform.position, Quaternion.identity);
                NetworkServer.Spawn(droppingGem);
                droppingGem.GetComponent<Rigidbody2D>().AddForce(GetExplosionForce() * GetExplosionDirection());
            }
        }

        private float GetExplosionForce()
        {
            return Random.Range(100f, maxExplosionForce);
        }

        private Vector2 GetExplosionDirection()
        {
            Vector2 randomDir = Vector2.zero;
            randomDir.x = Random.Range(-1f, 1f);
            randomDir.y = Random.Range(-1f, 1f);
            return randomDir.normalized;
        }
    }
}