using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class Ability : NetworkBehaviour
{
    [SerializeField] protected KeyCode _abilityKey;
    [SerializeField] protected UnityEvent _ability;
    [Range(0, 1000)]
    [SerializeField] protected float _abilityTime = 6f, _abilityCulldown = 3f;
    [SyncVar]
    protected float _activationTime, _waitingTime;

    [Server]
    public virtual void Start()
    {
        _activationTime = Time.time;
    }


    [Client]
    public virtual void FixedUpdate()
    {
        if (!isOwned) return;
        if (!Input.GetKeyUp(_abilityKey)) return;
        if (!IsOnCD() && !IsActivated()) return;
        UpdateActivationTime();
        InvokeAbilityCmd();
    }

    [ServerCallback]
    public virtual bool IsOnCD()
    {
        if (_waitingTime != 0) return Time.time - _waitingTime <= _abilityCulldown;
        else return false;
    }

    [ServerCallback]
    public virtual bool IsActivated()
    {
        if (_activationTime != 0) return Time.time - _activationTime <= _abilityTime;
        else return false;
    }

    [Command]
    public virtual void UpdateActivationTime()
    {
        _activationTime = Time.time;
    }

    [Command]
    public virtual void UpdateWaitingTime()
    {
        _waitingTime = Time.time;
    }

    [Command]
    public virtual void InvokeAbilityCmd()
    {
        InvokeAbility();
    }

    [Command]
    public virtual void StartContiniousAbility ()
    {
        StartCoroutine(ActivateContiniousAbility());
    }

    public virtual IEnumerator ActivateContiniousAbility ()
    {
        UpdateWaitingTime();
        yield return null;
    }
        


    [Client]
    public virtual void InvokeAbility ()
    {
        _ability.Invoke();
    }
}
    