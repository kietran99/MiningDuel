using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class PlayerExplosionHandler : NetworkBehaviour, IExplodable
    {
        private readonly float PERCENTAGE_MODIFIER = 0.01f;

        #region  SERIALIZE FIELDS
        [SerializeField]
        private GameObject droppingGemPrefab = null;

        [SerializeField]
        private float maxExplosionForce = 250f;

        [SerializeField]
        private SpriteRenderer playerRenderer = null;

        [SerializeField]
        private int numberOfFlashes = 3;

        [SerializeField]
        private float flashDelay = .075f;
        #endregion

        private ScoreManager scoreManager;
        private uint playerId;
        private Color originalPlayerColor;
        private WaitForSecondsRealtime flashDelayAsWaitForSeconds;

        void Start()
        {
            scoreManager = GetComponent<ScoreManager>();
            playerId = GetComponent<NetworkIdentity>().netId;
            originalPlayerColor = playerRenderer.color;
            flashDelayAsWaitForSeconds = new WaitForSecondsRealtime(flashDelay);
        }

        [Server]
        public void HandleExplosion(Transform throwerTransform, uint throwerID, float gemDropPercentage)
        {
            int dropAmount = Mathf.FloorToInt(scoreManager.CurrentMultiplier * scoreManager.CurrentScore * gemDropPercentage * PERCENTAGE_MODIFIER);
            RpcPlayDamagingEffect();

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

        [ClientRpc]
        private void RpcPlayDamagingEffect()
        {
            StopAllCoroutines();
            StartCoroutine(PlayDamagingEffect());
        }

        [Client]
        private System.Collections.IEnumerator PlayDamagingEffect()
        {
            int flashCnt = 0;

            while (flashCnt < numberOfFlashes)
            {               
                playerRenderer.color = Color.red;
                yield return flashDelayAsWaitForSeconds;
                playerRenderer.color = originalPlayerColor;
                flashCnt++;

                yield return flashDelayAsWaitForSeconds;
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
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                StopAllCoroutines();
                StartCoroutine(PlayDamagingEffect());
            }
        }
    }
}