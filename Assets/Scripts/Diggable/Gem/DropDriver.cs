using Mirror;
using UnityEngine;

namespace MD.Diggable.Gem
{
    [RequireComponent(typeof(DropObtain))]
    public class DropDriver : NetworkBehaviour
    {
        [SerializeField]
        private float driveSpeed = 10f;

        public Transform ThrowerTransform { get; set; }

        public override void OnStartServer()
        {
            GetComponent<Rigidbody2D>().simulated = true;          
        }

        [ServerCallback]
        private void Update()
        {
            DriveTowardsThrower();
        }

        private void DriveTowardsThrower()
        {    
            Vector2 movePos = new Vector2(ThrowerTransform.position.x - transform.position.x, 
                                            ThrowerTransform.position.y - transform.position.y);    
            transform.Translate(movePos * driveSpeed * Time.deltaTime);
        } 
    }
}
