using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MenuNetworkManager : NetworkManager
{
    private List<NetworkConnectionToClient> _connectedPlayers { get; } = new List<NetworkConnectionToClient>();
    private bool isGameInProgress = false;

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!isGameInProgress) return;
        _connectedPlayers.Add(conn);
        base.OnServerConnect(conn);
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        _connectedPlayers.Remove(conn);
        base.OnServerDisconnect(conn);
    }
    public override void OnStopServer()
    {
        isGameInProgress = false;
        _connectedPlayers.Clear();
    }
    public void StartGame (string _mapName)
    {
        if (_connectedPlayers.Count < 2) return;
        isGameInProgress = true;
        ServerChangeScene(_mapName);
    }
    public override void OnStopClient()
    {
        _connectedPlayers.Clear();
    }
}
