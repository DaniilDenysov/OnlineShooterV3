using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VampireUlt : Ability
{

    [SerializeField] private SynchronizedCustomSlider _slider;
    [Range(0,1)]
    [SerializeField] private float _multiplier;
    [SerializeField] private CustomNetworkPlayer _owner;
    private bool isActive=false;


    [Server]
    public void Start()
    {
        PlayerShootController.OnDamageDealt += PlayerShootController_OnDamageDealt;
    }

    [Client]
    public override void FixedUpdate()
    {
        if (!isOwned) return;
        if (!IsOnCD() && !IsActivated())
        {
            isActive = false;
            return;
        }
        if (!Input.GetKeyUp(_abilityKey)) return;
        isActive = true;
        UpdateActivationTime();
        InvokeAbilityCmd();
    }

    [Server]
    private void PlayerShootController_OnDamageDealt(string arg1, float arg2)
    {
        if (!isActive) return;
        if (arg1 != _owner.GetName()) return;
        _slider.SetCurrentValue(_slider.GetCurrentValue()+arg2);
    }
}
