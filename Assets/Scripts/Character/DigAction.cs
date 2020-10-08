using UnityEngine;

namespace MD.Character
{
    public class DigAction : MonoBehaviour
    {
        [SerializeField]
        private int power = 1;

        public int Power { get => power; set => power = value; }
    }
}