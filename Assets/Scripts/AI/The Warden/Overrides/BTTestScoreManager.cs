using UnityEngine;

namespace MD.AI.TheWarden
{
    public class BTTestScoreManager : MonoBehaviour, Character.IScoreManager
    {
        [SerializeField]
        private int score = 0;

        public int CurrentScore => score;
    }
}
