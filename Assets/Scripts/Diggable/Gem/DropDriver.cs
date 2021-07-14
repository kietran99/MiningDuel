using Mirror;
using UnityEngine;

namespace MD.Diggable.Gem
{
    [RequireComponent(typeof(DropObtain))]
    public class DropDriver : NetworkBehaviour
    {
        [SerializeField]
        private float driveSpeed = 10f;

        public Transform Attacker { get; set; }

        public override void OnStartServer()
        {
            GetComponent<Rigidbody2D>().simulated = true;          
        }

        [ServerCallback]
        private void Update()
        {
            if (Attacker == null)
            {
                Debug.LogError("Attacker is null");
                return;
            }

            DriveTowardsThrower();
        }

        private void DriveTowardsThrower()
        {    
            Vector2 movePos = new Vector2(Attacker.position.x - transform.position.x, 
                                            Attacker.position.y - transform.position.y);    
            transform.Translate(movePos * driveSpeed * Time.deltaTime);
        } 
    }
}
