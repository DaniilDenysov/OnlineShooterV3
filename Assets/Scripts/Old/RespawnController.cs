using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Mirror;
using System;

public class RespawnController : NetworkBehaviour
{
    [Header("On death")]
    [SerializeField] private UnityEvent onDeath;
    [Header("On respawn")]
    [SerializeField] private UnityEvent onRespawn;

    [SerializeField] private List<GameObject> _players = new List<GameObject>();

    [Header("Spawn settings")]
    [Range(0,1000)]
    [SerializeField] private float spawnTime = 60f;
    [SerializeField] private TMP_Text spawnTimeDisplay;
    private bool canRespawn;
    private int deathCount;
    private int _prevID;

    [ServerCallback]
    void Start()
    {
    //    PlayerHPController.OnPlayerDeath += OnPlayerDied;
        PlayerAction.PlayerDied += OnPlayerDied;
    }

    [ServerCallback]
    private void OnPlayerDied(NetworkConnectionToClient obj)
    {
        HandleDeath(obj);
    }

    /*private void OnPlayerDied(object sender, EventArgs e)
    {
        HandleDeath(((PlayerHPController)sender).GetComponent<NetworkIdentity>().connectionToClient);
    }*/

    [TargetRpc]
    private void HandleDeath (NetworkConnection targetClient)
    {
        deathCount++;
        OnDeathActions();
        StartCoroutine(Timer());
    }

    private void OnDeathActions ()
    {
        onDeath?.Invoke();
    }

    private void OnRespawnActions()
    {
        onRespawn?.Invoke();
    }

    public void ChangeState (GameObject obj)
    {
        obj.SetActive(!obj.active);
    }

    
    public void Respawn ()
    {
        if (!NetworkClient.isConnected) return;
        if (!canRespawn) return;
        OnCreateCharacter(_prevID);
        OnRespawnActions();
    }
    public void Respawn(int ID)
    {
        _prevID = ID;
        OnCreateCharacter(ID);
    }

    [Server]
    public override void OnStartServer()
    {
        CustomNetworkPlayer [] _palyers = FindObjectsOfType<CustomNetworkPlayer>();
        foreach (CustomNetworkPlayer _player in _palyers)
        {
            NetworkServer.Destroy(_player.gameObject);
        }
    }

    [Command(requiresAuthority = false)]
    public void OnCreateCharacter(int _playerID, NetworkConnectionToClient conn=null)
    {
        NetworkStartPosition [] _spawns = FindObjectsOfType<NetworkStartPosition>();
        GameObject _palyer = Instantiate(_players[_playerID]);
        _palyer.transform.position = _spawns[UnityEngine.Random.Range(0, _spawns.Length)].transform.position;
        NetworkServer.ReplacePlayerForConnection(conn, _palyer);
        _palyer.GetComponent<CustomNetworkPlayer>().SetName($"Player{conn.connectionId}");
        PlayerAction.OnPlayerAdded($"Player{conn.connectionId}");
    }

    IEnumerator Timer ()
    {
        canRespawn = false;
        float time = Time.time+spawnTime;
        while (time > Time.time)
        {
            spawnTimeDisplay.text = $"Time left {Mathf.Round((time - Time.time) * 10f) / 10f}";
            yield return null;
        }
        spawnTimeDisplay.text = $"You can respawn!";
        canRespawn = true;
    }
}