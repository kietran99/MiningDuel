using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;
namespace MD.Character
{
    [RequireComponent(typeof(PlayerBot))]
    public class BotExplosionHandler : MonoBehaviour, IExplodable
    {
        #region  SERIALIZE FIELDS
        [SerializeField]
        private GameObject droppingGemPrefab = null;

        [SerializeField]
        private float maxExplosionForce = 250f;
        #endregion

        private PlayerBot bot = null;
        private PlayerBot Bot
        {
            get
            {
                if (bot != null) return bot;
                return bot = GetComponent<PlayerBot>();
            }
        }

        [Server]
        public void HandleExplosion(uint throwerID, float gemDropPercentage, int bombType)
        {
            Debug.Log(transform.name + " was exploded");
            // if (!ServiceLocator.Resolve<IScoreManager>(out IScoreManager scoreManager)) return;

            int numOfGem = Mathf.FloorToInt(Bot.GetCurrentScore() * gemDropPercentage * .01f);
            Bot.DecreaseScore(numOfGem);

            for (int i = 0; i < numOfGem; i++)
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