using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Run : Movement
{
    [Range(0, 100)]
    [SerializeField] private float _runSpeed = 5f;

    [SerializeField] private KeyCode _runButton;

    [ClientCallback]
    private void Update()
    {
        if (!isOwned) return;
        Vector3 _velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (_velocity.magnitude == 0) return;
        if (!Input.GetKey(_runButton)) return;
        _velocity.Normalize();
        Move(_velocity * _runSpeed);
    }
}
