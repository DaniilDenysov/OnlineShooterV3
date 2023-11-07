using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Destroyer : NetworkBehaviour
{
    [Range(0, float.MaxValue)]
    [SerializeField] private float lifetime = 0.1f;

    [Server]
    private void Start()
    {
        if (lifetime == 0) return;
        Invoke(nameof(DestroyObject), lifetime);
    }
    [Server]
    private void DestroyObject()
    {
        NetworkServer.Destroy(this.gameObject);
    }
}
