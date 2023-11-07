using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillFeedController : NetworkBehaviour
{
    [SerializeField] private KillFeedItem killFeedPart;
    [SerializeField] private Transform killFreedPlaceHolder;

    private void Start()
    {
        PlayerAction.PlayerKill += OnPlayerKill;
    }

    private void OnPlayerKill(string arg1, string arg2, string arg3)
    {
        if (isClientOnly) return;
        DisplayKill(arg1, arg2, arg3);
    }

    [ClientRpc]
    private void DisplayKill (string arg1, string arg2, string arg3)
    {
        KillFeedItem item = Instantiate(killFeedPart);
        item.InitializeData(arg1,arg2,arg3);
        item.transform.SetParent(killFreedPlaceHolder);
    }
}
    