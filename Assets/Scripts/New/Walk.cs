using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Walk : Movement
{
    [Range(0, 100)]
    [SerializeField] private float _walkSpeed = 5f;
    private Vector3 _velocity;

    [Client]
    private void Update()
    {
        if (!isOwned) return;
        ChangeVelocity(new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")));
    }

    [Command]
    private void ChangeVelocity (Vector3 _newVelocity)
    {
        _velocity = _newVelocity;
        OnVelocityChanged();
    }

    [Server]
    private void FixedUpdate()
    {
        OnVelocityChanged();
    }

    [Server]
    private void OnVelocityChanged ()
    {
        if (!(_velocity.magnitude > 0))
        {
            Move(Vector3.zero);
            return;
        }
        _velocity.Normalize();
        Move(_velocity *= _walkSpeed);
    }
}
