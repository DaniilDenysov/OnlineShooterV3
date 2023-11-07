using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Mirror;


public class PostProcessingManager : NetworkBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private AnimationCurve _chromaticAberrationSettings;

   
    private ChromaticAberration _chromaticAberration;

    private void Awake()
    {
      //  if (!isOwned) Destroy(this);
        Ultimate.OnTimeChange += Ultimate_OnTimeChange1;
        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet(out _chromaticAberration);
       
    }

    private void Ultimate_OnTimeChange1(float arg1, float arg2, Vector3 arg3)
    {
        float _radius = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(arg3.x, 0, arg3.z));
        Debug.Log($"Radius:{_radius}");
        if (_radius > arg2) return;
        TimeChange(_chromaticAberrationSettings.Evaluate(arg1));
    }

    private void Ultimate_OnTimeChange(float obj)
    {
       // TimeChange(_chromaticAberrationSettings.Evaluate(obj));
    }

    [ClientRpc]
    private void TimeChange(float obj)
    {
        _chromaticAberration.intensity.value = obj;
    }
}
