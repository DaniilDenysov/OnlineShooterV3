using TMPro;
using UnityEngine;

public class ScoreboardPart : MonoBehaviour
{
    [SerializeField] private TMP_Text player;
    public void UpdateName(string player) => this.player.text = player;
    [SerializeField] private TMP_Text kills;
    public void UpdateKills(int kills) => this.kills.text = $"{kills}";
    [SerializeField] private TMP_Text deaths;
    public void UpdateDeaths(int deaths) => this.deaths.text = $"{deaths}";
}
