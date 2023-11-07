using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class Server : NetworkBehaviour
{
    public static Server Instance;
    [SerializeField] private GameObject playerRagdoll,_killFeedItem,_scoreBoardItem;

   

    private void Awake()
    {
        Instance = this;
    }

    [ServerCallback]
    private void Start()
    {
        PlayerHPController.OnPlayerDeath += OnPlayerDeath;
        PlayerAction.PlayerDied += OnPlayerDied;
    }

    [ServerCallback]
    private void OnPlayerDied(NetworkConnectionToClient obj)
    {
        if (!NetworkServer.active) return;
        InstantiateObject(playerRagdoll, obj.identity.gameObject.transform.position, obj.identity.gameObject.transform.rotation, obj);
        NetworkServer.Destroy(obj.identity.gameObject);
    }

    private void OnPlayerDeath(object sender, EventArgs e)
    {
        if (!NetworkServer.active) return;
        GameObject senderObject = ((PlayerHPController)sender).gameObject;
        NetworkServer.Destroy(((Transform)sender).gameObject);
    }

    [ClientRpc]
    public void PlayerChangedState (int connectionID,bool state)
    {
        if (NetworkServer.connections.TryGetValue(connectionID, out var value))
        {
            Debug.Log(value);
            value.identity.gameObject.SetActive(state);
        }
    }

    [ServerCallback]
    public static void InstantiateObject (GameObject prefab,Vector3 position, Quaternion rotation, NetworkConnectionToClient caller)
    {
        GameObject instantiatedPrefab = Instantiate(prefab,position,rotation);
        NetworkServer.Spawn(instantiatedPrefab, caller);
    }
    [Server]
    public static void DestroyObject(GameObject objectToDestroy)
    {
        NetworkServer.Destroy(objectToDestroy);
    }
}
