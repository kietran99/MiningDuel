using Mirror;
using UnityEngine;
using MD.Diggable.Core;
using System.Collections;

namespace MD.Quirk
{
    public class DrillMachine : BaseQuirk, MD.Diggable.Projectile.IExplodable 
    {
        [SerializeField]
        private int drillPower = 9999;

        [SerializeField]
        private int diggingRadius = 3;
        
        [SerializeField]
        private float drillDelay = 2f;

        private readonly float GRID_OFFSET = .5f;
        private bool shouldDestroy = false;

        private DiggableType typeToDig;

        private Vector2Int[] digArea;
        private IDiggableGenerator diggableGenerator; 

        private bool isInitialized = false;

        // public int maxUses = 3; 
        // int usesLeft = 0;

        // public override void OnStartServer()
        // {
        //     usesLeft = maxUses;
        // }

        public override void SyncActivate(NetworkIdentity user)
        {
            base.SyncActivate(user);
            if (hasAuthority) return;

            System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + GRID_OFFSET;
            transform.position = new Vector3(SnapPosition(user.transform.position.x), SnapPosition(user.transform.position.y), 0f);
            transform.parent = null;
        }

        public override void SingleActivate(NetworkIdentity user)
        {
            System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + GRID_OFFSET;
            transform.position = new Vector3(SnapPosition(user.transform.position.x), SnapPosition(user.transform.position.y), 0f);
            transform.parent = null;
            CmdInitializeData();
            StartCoroutine(StartDrilling(user));
        }

        private void SetDiggableType()
        {
            typeToDig = DiggableType.COMMON_GEM;
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
        private void CmdInitializeData()
        {
            if (!ServiceLocator.Resolve(out diggableGenerator))
            {
                Debug.LogError("cant find diggable generator");
                return;
            }
            SetDiggableType();
            Vector2Int centerSquare= new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            Vector2Int position  = Vector2Int.zero;
            System.Collections.Generic.List<Vector2Int> area = new System.Collections.Generic.List<Vector2Int>();
            for (int x = -diggingRadius; x <= diggingRadius; x++)
            {
                for (int y = -diggingRadius; y<= diggingRadius; y++)
                {
                    position = centerSquare + new Vector2Int(x,y);
                    area.Add(position);
                }
            }
            digArea = area.ToArray();
            isInitialized = true;
        }

        [Command]
        private void CmdRequestDrill(NetworkIdentity user)
        {
            if (!isInitialized) return;

            if (!GetDiggablePosition(out Vector2Int currentTarget))
            {
                Debug.Log("no " + typeToDig + " in area");
                return;
            }
            diggableGenerator.DigAt(
                            user, 
                            currentTarget.x, 
                            currentTarget.y, 
                            drillPower)                
            ;
        }

        public void HandleExplosion(Transform throwerTransform, uint throwerID, float gemDropPercentage)   => HandleDestroy();
        public void HandleTrapExplode(float slowDownTime) => HandleDestroy();


        public void HandleDestroy()
        {
            Debug.Log("Drill Machine Detroyed");
            StopAllCoroutines();
            shouldDestroy = true;
            Destroy(gameObject);
        }

        [Server]
        private bool GetDiggablePosition(out Vector2Int pos)
        {
            if (diggableGenerator == null) Debug.LogError(" null digg");
            pos = Vector2Int.zero;
            DiggableType[] diggableInfosArr = diggableGenerator.GetDiggableArea(digArea);
            for (int i=0; i < diggableInfosArr.Length; i++)
            {
                if (diggableInfosArr[i].Equals(typeToDig))
                {
                    pos = digArea[i];
                    return true;
                }
            }
            return false;
        }

    }
}
