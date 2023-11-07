using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class PlayerHealthSystem : HealthSystem
{
    [ServerCallback]
    public override bool OnDamage(float damage)
    {
        _healthPoints.SetCurrentValue(_healthPoints.GetCurrentValue()-damage);
        if (_healthPoints.GetCurrentValue() > 0) return false;
        PlayerAction.OnPlayerDied(connectionToClient);
        return true;
    }
}
