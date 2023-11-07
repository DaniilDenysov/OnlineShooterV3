using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PositionHolder : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private Transform fixedPosition;

    [Command]
    public void SetFixedPosition (Transform fixedPosition)
    {
        this.fixedPosition = fixedPosition;
    }

    private void FixedUpdate()
    {
        if (fixedPosition == null) return;
        transform.position = fixedPosition.position;
    }
}
