using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Mirror;

public class DecalController : NetworkBehaviour
{
    [SerializeField] private List<Material> decals = new List<Material>();
    [SerializeField] private AnimationCurve faideAmount;

    private float startTime = 0;
    private DecalProjector decalProjector;

    [Server]
    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
        startTime = (float)NetworkTime.time;
        if (decals.Count == 0) return;
        decalProjector.material = decals[Random.Range(0, decals.Capacity - 1)];
    }
    [Server]
    void Update()
    {
        decalProjector.fadeFactor = faideAmount.Evaluate(((float)NetworkTime.time) - startTime);
       // transform.localScale = new Vector3(value,value,value);
    }
}
