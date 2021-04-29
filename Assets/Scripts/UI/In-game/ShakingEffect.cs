using UnityEngine;

namespace MD.UI
{
    public class ShakingEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject simulatingGO = null;

        [SerializeField]
        private float simulatingSeconds = 1f;

        [SerializeField]
        private float magnitudeX = 1f;

        [SerializeField]
        private float magnitudeY = 1f;

        private Vector3 originalPos;

        private void Start()
        {
            originalPos = simulatingGO.transform.position;
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<Character.HPChangeData>(PlayIfTakingDamage);           
        }

        private void PlayIfTakingDamage(Character.HPChangeData data)
        {
            if (data.lastHP > data.curHP)
            {
                simulatingGO.transform.position = originalPos;
                StopAllCoroutines();
                StartCoroutine(PlayTakeDamageVFX());
            }
        }
        
        private System.Collections.IEnumerator PlayTakeDamageVFX()
        {
            var elapsed = 0f;

            while (elapsed <= simulatingSeconds)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * magnitudeX;
                float y = UnityEngine.Random.Range(-1f, 1f) * magnitudeY;
                simulatingGO.transform.position += new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;

                yield return null;
            }

            simulatingGO.transform.position = originalPos;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                simulatingGO.transform.position = originalPos;
                StopAllCoroutines();
                StartCoroutine(PlayTakeDamageVFX());
            }
        }
    }
}
