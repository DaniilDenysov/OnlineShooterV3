using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class CustomNetworkManager : NetworkManager
{ 
    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        CustomNetworkPlayer player = conn.identity.GetComponent<CustomNetworkPlayer>();
        if (!player.isEmptyPlayer())
        {
            player.SetName($"Player{conn.connectionId}");
             PlayerAction.OnPlayerAdded($"Player{conn.connectionId}");
        }
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        PlayerAction.OnPlayerRemoved($"Player{conn.connectionId}");
    }
}