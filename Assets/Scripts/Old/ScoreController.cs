using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ScoreController : NetworkBehaviour
{

    [SerializeField] private KeyCode key = KeyCode.Tab;
    [SerializeField] private List<GameObject> board = new List<GameObject>();
    [SerializeField] private GameObject scoreboardRow;
    [SerializeField] private Transform scoreboard;
    public static ScoreController Instance;
    private SyncDictionary<string, PlayerStatistics> _scoreboard = new SyncDictionary<string, PlayerStatistics>();


    struct PlayerStatistics
    {
        public int _kills, _deaths;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerAction.PlayerKill += OnPlayerKill;
        PlayerAction.PlayerAdded += OnPlayerAdded;
        PlayerAction.PlayerRemoved += OnPlayerRemoved;
    }

    [ClientRpc]
    public void ClearBoard()
    {
        Debug.Log("Clearing board");
        foreach (GameObject g in board)
        {
            Destroy(g);
        }
        board.Clear();
    }

    [ClientRpc]
    public void UpdateBoard()
    {
        Debug.Log("Updating board");
        foreach (KeyValuePair<string, PlayerStatistics> pair in _scoreboard)
        {
            Debug.Log($"Adding {pair.Key} {pair.Value._deaths} {pair.Value._kills}");
            AddScoreboardPart(pair.Key, pair.Value._kills, pair.Value._deaths);
        }
    }

    private void OnPlayerRemoved(string e)
    {
        if (isClientOnly) return;
        _scoreboard.Remove(e);
    }


    private void OnPlayerAdded(string e)
    {
        if (isClientOnly) return;
        PlayerStatistics playerStatistics = new PlayerStatistics();
        _scoreboard.TryAdd(e, playerStatistics);
        ClearBoard();
        UpdateBoard();
    }


    private void OnPlayerKill(string arg1, string arg2, string arg3)
    {
        if (isClientOnly) return;
        PlayerStatistics _killer = new PlayerStatistics(), _victim = new PlayerStatistics();
        _scoreboard.TryAdd(arg1, _killer);
        _scoreboard.TryAdd(arg2, _victim);
        if (_scoreboard.TryGetValue(arg1, out _killer))
        {
            _killer._kills++;
            _scoreboard.Remove(arg1);
            _scoreboard.Add(arg1, _killer);
        }
        if (_scoreboard.TryGetValue(arg2, out _victim))
        {
            _victim._deaths++;
            _scoreboard.Remove(arg2);
            _scoreboard.Add(arg2, _victim);
        }
        ClearBoard();
        UpdateBoard();
    }

    private void Update()
    {
        if (!Input.GetKey(key)) { scoreboard.gameObject.SetActive(false); return; }
        scoreboard.gameObject.SetActive(true);
    }

    public void AddScoreboardPart(string name, int k, int d)
    {
        GameObject tmp = Instantiate(scoreboardRow);
        tmp.transform.SetParent(scoreboard);
        ScoreboardPart scr = tmp.GetComponent<ScoreboardPart>();
        scr.UpdateName(name);
        scr.UpdateKills(k);
        scr.UpdateDeaths(d);
        board.Add(tmp);
    }
}
