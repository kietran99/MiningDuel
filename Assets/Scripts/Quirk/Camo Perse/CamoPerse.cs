using UnityEngine;

namespace MD.Quirk
{
    public class CamoPerse : BaseQuirk
    {
        private readonly float GRID_OFFSET = .5f;

        [SerializeField]
        private float armSeconds = 1f;

        public override void Activate()
        {
            ServiceLocator
                .Resolve<Character.Player>()
                .Match(
                    err => Debug.LogError(err.Message),
                    player => 
                    {
                        transform.position = 
                            new Vector3(
                                Mathf.FloorToInt(player.transform.position.x) + GRID_OFFSET,
                                Mathf.FloorToInt(player.transform.position.y) + GRID_OFFSET,
                                0f
                            );
                    }
                );
        }
    }
}
