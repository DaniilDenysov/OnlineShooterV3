using System;
using UnityEngine;
using Mirror;

public class PlayerHPController : NetworkBehaviour,IDamagable
{
    [SerializeField] private SynchronizedCustomSlider HP;

    public static EventHandler OnPlayerDeath;

    [ContextMenu("DoDamage")]
    public void Test ()
    {
        OnDamage(100);
    }
 
    [ServerCallback]
    public bool OnDamage (float damage)
    {
        HP.SetCurrentValue(HP.GetCurrentValue()-damage);
        if (HP.GetCurrentValue() == 0)
        {
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            return true;
        }
        Debug.Log($"Damage{damage}");
        return false;
    }

    public float GetHealth()
    {
        return HP.GetCurrentValue();
    }
}
