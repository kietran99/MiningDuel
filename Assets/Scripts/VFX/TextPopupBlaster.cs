using UnityEngine;

namespace MD.VisualEffects
{
    public class TextPopupBlaster : MonoBehaviour
    {
        [SerializeField]
        private float lifeTime = 1f;

        [SerializeField]
        private Rigidbody2D rigidBody = null;
        
        [SerializeField]
        private MeshRenderer meshRenderer = null;

        [SerializeField]
        private string sortingLayer = "Character";

        [SerializeField]
        private int sortingOrder = 11;

        [SerializeField]
        private TextMesh textMesh = null;

        [SerializeField]
        private float minSize = .1f;

        [SerializeField]
        private float maxSize = .15f;

        public System.Action<GameObject> OnFade { get; set; }

        private void Awake()
        {
            meshRenderer.sortingLayerName = sortingLayer;
            meshRenderer.sortingOrder = sortingOrder;
        }

        public void Blast(string value, Vector2 pos, Color color)
        {
            textMesh.color = color;
            textMesh.text = value;
            textMesh.characterSize = minSize;
            transform.position = pos;
            rigidBody.simulated = true;
            rigidBody.AddForce(RandomExplosionForce() * RandomExplosionDirection(), ForceMode2D.Impulse);
            StartCoroutine(EnlargeThenSmallen());
        }

        private System.Collections.IEnumerator EnlargeThenSmallen()
        {
            var elapsed = 0f;
            var shouldEnlarge = true;

            while (elapsed < lifeTime)
            {
                if (shouldEnlarge)
                {
                    if (textMesh.characterSize >= maxSize)
                    {
                        shouldEnlarge = false;
                        continue;
                    }

                    textMesh.characterSize += Time.deltaTime;
                    continue;
                }

                if (textMesh.characterSize > minSize)
                {
                    textMesh.characterSize -= Time.deltaTime;
                }

                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - Time.deltaTime);
                elapsed += Time.deltaTime;

                yield return null;
            }

            OnFade?.Invoke(gameObject);
        }

        private float RandomExplosionForce() => Random.Range(5f, 7f);

        private Vector2 RandomExplosionDirection() => new Vector2(Random.Range(-.5f, .5f), Random.Range(0f, 1f)).normalized;
    }
}
