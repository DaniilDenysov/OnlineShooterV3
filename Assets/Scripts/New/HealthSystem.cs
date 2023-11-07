using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class HealthSystem : NetworkBehaviour,IDamagable
{
    [SerializeField] protected SynchronizedCustomSlider _healthPoints;

    [ContextMenu("DoDmg")]
    public virtual void DoDamage ()
    {
        CmdDoDamage();
    }

    [Command]
    public virtual void CmdDoDamage ()
    {
        OnDamage(100f);
    }

    [ServerCallback]
    public virtual bool OnDamage(float damage)
    {
        _healthPoints.SetCurrentValue(_healthPoints.GetCurrentValue()-damage);
        return !(_healthPoints.GetCurrentValue() > 0);
    }

}
