using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarController : MonoBehaviour
{
    Transform player = null;

    void LateUpdate()
    {
        if (player == null)
        {
            if (!ServiceLocator.Resolve(out MD.Character.Player player)) return;
            this.player = player.transform;
        }
        transform.position = player.position;
    }
}
