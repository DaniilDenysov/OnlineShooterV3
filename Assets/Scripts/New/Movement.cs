using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(Rigidbody))]
public class Movement : NetworkBehaviour
{
    private Rigidbody _rigidbody;

    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    [Command(requiresAuthority =false)]
   public virtual void Move (Vector3 _direction)
   {
        _rigidbody.velocity = _direction;
   }
}
