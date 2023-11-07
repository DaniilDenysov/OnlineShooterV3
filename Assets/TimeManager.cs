using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class TimeManager : NetworkBehaviour
{
    public static TimeManager Instance;
    public static event UnityAction<float> OnTimeChange;
    [SerializeField] private bool _isTolerant;

    [SerializeField]
    [SyncVar(hook = nameof(TimeChanged))] private float _time = 1f;

    private void Awake()
    {
        Instance = this;
    }


    private void TimeChanged (float newT, float oldT)
    {
       if (_isTolerant) return;
        Time.timeScale = newT;
        Debug.Log($"New:{newT} Old:{oldT}");
    }

   


    private void Start()
    {
        OnTimeChange += TimeManager_OnTimeChange;
    }


    private void TimeManager_OnTimeChange(float arg0)
    {
        CmdTimeChange(arg0);
    }
    
    public void CmdTimeChange (float _time)
    {
        this._time = _time;
    }


    public void SetTolerant (bool _isTolerant)
    {
        this._isTolerant = _isTolerant;
    }
}
