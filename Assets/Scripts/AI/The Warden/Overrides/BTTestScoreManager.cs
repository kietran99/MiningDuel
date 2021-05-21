using System;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class BTTestScoreManager : MonoBehaviour, Character.IScoreManager, IWardenDamagable
    {
        [SerializeField]
        private int score = 0;

        public int CurrentScore => score;

        public Action<uint> OnDeath { get; set; }

        public void TakeWardenDamage(int dmg)
        {
            Debug.Log("Took " + dmg + " from The Warden");
        }
    }
}
