using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialDropDriver : MonoBehaviour
    {
        [SerializeField]
        private float driveSpeed = 10f;

        [SerializeField]
        private int dropValue = 1;

        public Transform ThrowerTransform { get; set; }

        private static int finalScore = 0;

        private void Start()
        {
            GetComponent<Rigidbody2D>().simulated = true;
        }

        void Update()
        {
            Vector2 movePos = new Vector2(ThrowerTransform.position.x - transform.position.x, 
                                            ThrowerTransform.position.y - transform.position.y);    
            transform.Translate(movePos * driveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }
         
            finalScore += dropValue;
            EventSystems.EventManager.Instance.TriggerEvent(new Character.FinalScoreChangeData(finalScore));
            Destroy(gameObject);
        }
    }
}
