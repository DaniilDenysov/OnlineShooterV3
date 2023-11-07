using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Destroyable : NetworkBehaviour,IDamagable
{
    [SerializeField] private SynchronizedCustomSlider slider;

    [Server]
    public void DestroyObject ()
    {
        if (slider.GetCurrentValue() <= 1) NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void SpawnObject (GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab);
        NetworkServer.Spawn(newObject, connectionToClient);
    }

    [ServerCallback]
    public bool OnDamage (float damage)
    {
        slider.SetCurrentValue(slider.GetCurrentValue()-damage);
        return false;
    }

    public float GetHealth()
    {
        return slider.GetCurrentValue();
    }
}
