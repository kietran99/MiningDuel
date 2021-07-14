using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Diggable.Core;

namespace MD.Quirk
{
    public class MightyBlessing : BaseQuirk
    {
        [SerializeField]
        private int radius = 5;

        [SerializeField]
        private int power = 9999;

        [SerializeField]
        private LayerMask affectLayers = 0;

        [SerializeField]
        private ParticleSystem vfx = null;

        [SerializeField]
        private float delayStartLevel = 2f;

        public override void ServerActivate(NetworkIdentity user)
        {
            Physics2D
                .OverlapCircleAll(user.transform.position, radius, affectLayers)
                .ForEach(collider =>
                {
                    if (!collider.TryGetComponent<Character.IDamagable>(out var damagable))
                    {
                        return;
                    }

                    if (collider.gameObject.GetInstanceID() == user.gameObject.GetInstanceID())
                    {
                        return;
                    }

                    damagable.TakeDamage(user, power, false);
                });

            base.ServerActivate(user);
        }

        public override void SyncActivate(NetworkIdentity user)
        {
            base.SyncActivate(user);
            transform.position = user.transform.position;
            vfx.Play();
            EventSystems.EventManager.Instance.TriggerEvent(new MightyBlessingActivateData());
        }

    #region LEGACY CODE
        private Vector2Int center;

        // public override void SingleActivate(NetworkIdentity userIdentity)
        // {
        //     center = new Vector2Int (
        //         Mathf.FloorToInt(userIdentity.transform.position.x),
        //         Mathf.FloorToInt(userIdentity.transform.position.y)
        //     );
      
        //     CmdRequestGemPos(userIdentity, center);
        // }

        [Command]
        private void CmdRequestGemPos(NetworkIdentity user, Vector2Int center)
        {
            Vector2Int[] digArea = GenCircularScannablePositions(center, radius);
            List<Vector2Int> posToDig = new List<Vector2Int>();
            ServiceLocator.Resolve(out IDiggableGenerator diggableGenerator);

            foreach (Vector2Int pos in digArea)
            {
                if (diggableGenerator.IsGemAt(pos.x, pos.y).Match(err => false, isProjAt => isProjAt))
                {
                    posToDig.Add(pos);
                }
            }

            TargetStartDig(user, posToDig.ToArray());
        }

        [TargetRpc]
        private void TargetStartDig(NetworkIdentity user, Vector2Int[] posToDig)
        {
            StartCoroutine(DigAllInRange(user, posToDig));
        }

        private IEnumerator DigAllInRange(NetworkIdentity userIdentity, Vector2Int[] posToDig)
        {
            float currentDelay = delayStartLevel;

            for (int i= 0; i < posToDig.Length; i++)
            {
                CmdDig(userIdentity, posToDig[i]);
                yield return new WaitForSeconds(1/Mathf.Exp(currentDelay));
                currentDelay +=.2f;  
            }

            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdDig(NetworkIdentity userIdentity, Vector2Int pos){
            ServiceLocator
            .Resolve<IDiggableGenerator>()
            .Match(
                unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                diggableGenerator => 
                    diggableGenerator.DigAt(
                        userIdentity, 
                        pos.x, 
                        pos.y,
                        power)                
            );
        }

        private Vector2Int[] GenCircularScannablePositions(Vector2Int center,int scanRange)
        {
            var temp = new List<Vector2Int>();
            temp.AddRange(GenGemPositions(center,scanRange));
            temp.AddRange(GenFillerPositions(center, scanRange));

            foreach (Vector2Int pos in temp)
            {
                Vector3 pos3d = new Vector3(pos.x,pos.y,-1f);
                Debug.DrawLine((Vector2)pos , pos + Vector2.one/10f, Color.green,5f);
            }

            return temp.ToArray();
        }

        private List<Vector2Int> GenGemPositions(Vector2Int center, int range)
        {
            var res = new List<Vector2Int>();

            for (int x = -range; x <= range; x++)
            {
                var yRange = range - Mathf.Abs(x);

                for (int y = -yRange; y <= yRange; y++)
                {
                    res.Add(new Vector2Int(x, y) + center);
                }
            }

            return res;
        }

        private List<Vector2Int> GenFillerPositions(Vector2Int center, int range)
        {
            var res = new List<Vector2Int>();
            var fillerRange = range + 1;

            for (int x = -fillerRange ; x <= fillerRange; x++)
            {
                var y = fillerRange - Mathf.Abs(x);

                if (y == 0 || y == fillerRange) continue;
                res.Add(new Vector2Int(x, y) + center);
                res.Add(new Vector2Int(x, -y) + center);
            }

            return res;
        }
    }
    #endregion
}