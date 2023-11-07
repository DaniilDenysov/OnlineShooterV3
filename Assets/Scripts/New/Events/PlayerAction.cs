using System.Collections;
using System.Collections.Generic;
using System;
using Mirror;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public static event Action<string,string,string> PlayerKill;
    public static event Action<NetworkConnectionToClient> PlayerDied;
    public static event Action<string> PlayerAdded, PlayerRemoved;

    public static void OnPlayerKill (string arg1, string arg2, string arg3)
    {
        PlayerKill.Invoke(arg1, arg2, arg3);
    }

    public static void OnPlayerDied(NetworkConnectionToClient arg1)
    {
        PlayerDied.Invoke(arg1);
    }

    public static void OnPlayerAdded(string arg1)
    {
        PlayerAdded.Invoke(arg1);
    }
    public static void OnPlayerRemoved(string arg1)
    {
        PlayerRemoved.Invoke(arg1);
    }
}
