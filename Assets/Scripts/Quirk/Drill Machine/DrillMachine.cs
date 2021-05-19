using Mirror;
using UnityEngine;
using MD.Diggable.Core;
using System.Collections;

namespace MD.Quirk
{
    public class DrillMachine : BaseQuirk, MD.Diggable.Projectile.IExplodable 
    {
        [SerializeField]
        private int drillPower = 5;
        
        [SerializeField]
        private float drillDelay = 2f;

        private readonly float GRID_OFFSET = .5f;
        private bool shouldDestroy = false;

        // public int maxUses = 3; 
        // int usesLeft = 0;

        // public override void OnStartServer()
        // {
        //     usesLeft = maxUses;
        // }

        public override void SyncActivate(NetworkIdentity user)
        {
            base.SyncActivate(user);
            System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + GRID_OFFSET;
            transform.position = new Vector3(SnapPosition(user.transform.position.x), SnapPosition(user.transform.position.y), 0f);
        }

        public override void SingleActivate(NetworkIdentity user)
        {             
            StartCoroutine(StartDrilling(user));
        }      

        private IEnumerator StartDrilling(NetworkIdentity user)
        {
            while(!shouldDestroy)
            {                            
                CmdRequestDrill(user);
                
                yield return new WaitForSeconds(drillDelay);
            }
        }

        [Command]
        private void CmdRequestDrill(NetworkIdentity user)
        {
            if (!GetClosestDiggable(out Vector2 currentTarget))
            {
                return;
            }

            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    diggableGenerator => 
                        diggableGenerator.DigAt(
                            user, 
                            Mathf.FloorToInt(currentTarget.x), 
                            Mathf.FloorToInt(currentTarget.y), 
                            drillPower)                
                );
        }

        public void HandleExplosion(Transform throwerTransform, uint throwerID, float gemDropPercentage)   => HandleDestroy();
        public void HandleTrapExplode(float slowDownTime) => HandleDestroy();


        public void HandleDestroy()
        {
            Debug.Log("Drill Machine Exploded");
            StopAllCoroutines();
            shouldDestroy = true;
            Destroy(gameObject);
        }

        [Server]
        private bool GetClosestDiggable(out Vector2 pos)
        {
            Vector2 sqrCenter = new Vector2(Mathf.FloorToInt(transform.position.x) + .5f, Mathf.FloorToInt(transform.position.y) + .5f);

            if (IsGemAt(sqrCenter)) 
            {
                pos = sqrCenter;
                return true;
            } 

            Vector2 position = Vector2.zero;

            for (int i = 1; i <= 3; i++)
            {
                for (int x = -i; x <= i; x++)
                {
                    for (int y = -i; y <= i; y++)
                    {
                        if (x != i && x != -i && y != i && y != -i) continue;

                        position = sqrCenter + new Vector2((float)x, (float)y);

                        if (IsGemAt(position))
                        {
                            pos = position;
                            return true;
                        }
                    }
                }
            }

            pos = default;
            return false;
        }

        [Server]
        private bool IsGemAt(Vector2 pos)
        {
            if (!ServiceLocator.Resolve(out IDiggableGenerator diggableGenerator))
            {
                return false;
            }

            return diggableGenerator.IsGemAt(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)).Match(err => false, isProjAt => isProjAt);
        }
    }
}
