using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerClockSynchronizer))]
public class TimeULT : Ability
{
    [SerializeField] private AnimationCurve _timeSlope;
    private bool isActive = false;

    [Range(0.1f, 100f)]
    [SerializeField] private float _abilityRange = 10;

    [Client]
    public override void Start()
    {
        GetComponent<PlayerClockSynchronizer>().SetTolerant(true);
    }

    [Client]
    public override void FixedUpdate()
    {
        if (!isOwned) return;
        if (isActive)
        {
            float value = _timeSlope.Evaluate((Time.time - _activationTime) / _abilityTime);
          //  TimeManager.Instance.CmdTimeChange(value);
            Ultimate.TimeChange(value,transform.position,_abilityRange);
            Debug.Log(value);
        }
        if (!IsActivated())
        {
            isActive = false;
        }
        if (!Input.GetKeyUp(_abilityKey)) return;
        if (isActive==true) return;
        isActive = true;
        UpdateActivationTime();
    }


}
