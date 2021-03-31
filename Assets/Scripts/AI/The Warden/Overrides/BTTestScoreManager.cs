using UnityEngine;

namespace MD.AI.TheWarden
{
    public class BTTestScoreManager : MonoBehaviour, Character.IScoreManager, IWardenDamagable
    {
        [SerializeField]
        private int score = 0;

        public int CurrentScore => score;

        public void TakeDamage()
        {
            Debug.Log(name + " was attacked by The Warden");
        }
    }
}
