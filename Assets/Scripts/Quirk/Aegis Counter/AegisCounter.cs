using Mirror;
using UnityEngine;

namespace MD.Quirk
{
    public class AegisCounter : BaseQuirk
    {
        [SerializeField] float expireTime = 120f;
        [SerializeField] [Range(0,90)] float angleDifference = 15;
        private bool shouldDestroy = false;
        private Transform player = null;
        public override void SyncActivate(NetworkIdentity userIdentity)
        {
            base.SyncActivate(userIdentity);
            player = userIdentity.transform;
            transform.position = player.position;
            transform.parent = player;
            Debug.Log("Quirk: Barrier Activated");
            Invoke(nameof(ExpireBarrier), expireTime);
        }
        void ExpireBarrier()
        {
            if(!shouldDestroy)
            {
                Debug.Log("Quirk: Barrier Destroyed");
                NetworkServer.Destroy(gameObject);
                shouldDestroy = true;
            }
        }
        private void FixedUpdate()
        {
            transform.localPosition = Vector3.zero;
        }
        private bool CheckBombThrown(Vector3 bomb, float radius)
        {
            float distance = (bomb - transform.position).magnitude;
            if(distance > (radius + GetComponent<CircleCollider2D>().radius))
            {
                return true;
            }
            return false;
        }
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            } 
            if(other.gameObject.GetComponent<MD.Diggable.Projectile.Explosion>() == null) 
            {
                return;
            }
            if(other.GetComponent<MD.Diggable.Projectile.ProjectileLauncher>().BeingHeld)
            {
                return;
            }
            other.GetComponent<MD.Diggable.Projectile.Explosion>().StopExplosion();
            var rb = other.GetComponent<Rigidbody2D>();
            if(rb == null) return;
            float speed = rb.velocity.magnitude;
            Debug.Log("Incoming"+rb.velocity);
            // Vector2 contact = Intersection(transform.position.x,transform.position.y,GetComponent<CircleCollider2D>().radius,rb.velocity);
            // Vector2 reflectDirection =  Vector2.Reflect(rb.velocity.normalized, contact.normalized);
            // Debug.Log(contact);
            // rb.velocity = reflectDirection*Mathf.Max(speed,0f);
            float angleDiff = DegreeToRadian(Random.Range(-angleDifference,angleDifference));
            float x1 = -Mathf.Cos(angleDiff)*rb.velocity.x + Mathf.Sin(angleDiff) * rb.velocity.y;
            float y1 = -Mathf.Sin(angleDiff)*rb.velocity.x - Mathf.Cos(angleDiff) * rb.velocity.y;
            rb.velocity = new Vector2(x1,y1); 
            Debug.Log("Outcoming" + rb.velocity);
            ExpireBarrier();
        }

        float DegreeToRadian(float degree)
        {
            return degree*3.14f/180f;
        }
        float ApproxSquareRoot(float num)
        {
            float res = 0;
            int i;
            for(i = 0;i*i <= num; i++)
            {
                if(i*i == num) return i;
            }
            float bot = 2*i - 1;
            float top = num - i*i;
            res = (float)i + top/bot;
            return res;
        }
        Vector2 Intersection(float cx, float cy, float radius, Vector2 incomingVector)
        {
            Vector2 res = Vector2.zero;
            //I'm sorry that you have to read this
            float a = incomingVector.y;
            float b = incomingVector.x;
            float c = a * cx - b * cy;
            float aSlash = Mathf.Pow(a,2) + Mathf.Pow(b,2);
            float bSlash = Mathf.Pow(b,2)*cx + a * c + a *cy;
            float cSlash = (Mathf.Pow(cx*b,2)+Mathf.Pow(c+cy,2)-Mathf.Pow(b*radius,2));
            float rootDelta = ApproxSquareRoot(Mathf.Pow(bSlash,2) - aSlash*cSlash);
            // There are confusing times
            float x1 = (-bSlash + rootDelta)/a;
            float x2 = (-bSlash - rootDelta)/a;
            float y1 = (a * x1 -c)/b;
            if((( x1>=cx && x1 <=incomingVector.x)||(x1<=cx && x1 >=incomingVector.x)) && 
                (( y1>=cy && y1 <=incomingVector.y)||(y1<=cy && y1 >=incomingVector.x)))
            {
                res.x = x1;
                res.y = y1;
                return res;
            }
            float y2 = (a * x2 -c)/b;
            res.x = x2;
            res.y = y2;
            return res;
        }
    }
}

