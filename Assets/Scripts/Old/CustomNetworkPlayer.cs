using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CustomNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    [SerializeField] private string playerName = "Player";
    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private bool _emptyPlayer;



    #region Server
    [Server]
    public void SetName(string playerName)
    {
        this.playerName = playerName;
    }
    [Command]
    private void CmdSetName(string newName)
    {
        RpcLogNewName(newName);
        SetName(newName);
    }

    [ServerCallback]
    public bool isEmptyPlayer() => _emptyPlayer;

    #endregion
    #region Client

    private void OnPlayerNameChanged (string oldName,string newName)
    {
        playerNameText.text = newName;
    }

    [ContextMenu("SetName")]
    private void SetName ()
    {
        CmdSetName("New");
    }

    [ClientCallback]
    public void RemoveUI() => Destroy(GetComponentInChildren<PlayerUIController>().gameObject);

    public string GetName() => playerName;
    [ClientRpc]
    private void RpcLogNewName (string name)
    {
        Debug.Log(name);
    }
    #endregion
}
