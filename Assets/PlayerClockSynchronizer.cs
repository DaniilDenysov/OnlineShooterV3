using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerClockSynchronizer : NetworkBehaviour
{
    [SerializeField] private bool _isTolerant;

    [SerializeField]
    [SyncVar(hook = nameof(TimeChanged))] private float _time = 1f;


    private void Awake()
    {
        Ultimate.OnTimeChange += Ultimate_OnTimeChange;
    }

    public void SetTolerant(bool _isTolerant)
    {
        this._isTolerant = _isTolerant;
    }

    private void TimeChanged(float newT, float oldT)
    {
        if (!isOwned) return;
        if (_isTolerant) return;
        Time.timeScale = newT;
        Debug.Log($"New:{newT} Old:{oldT}");
    }

    private void Ultimate_OnTimeChange(float arg1, float arg2, Vector3 arg3)
    {
        if (!isOwned) return;
        if (_isTolerant) return;
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(arg3.x, 0, arg3.z)) > arg2) return;
        _time = arg1;
    }
}
