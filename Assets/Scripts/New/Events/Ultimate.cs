using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Ultimate : MonoBehaviour
{
    public static event Action<float,float,Vector3> OnTimeChange;

    public static void TimeChange (float _time,Vector3 _position, float _range)
    {
        OnTimeChange?.Invoke(_time,_range,_position);
    }
}
