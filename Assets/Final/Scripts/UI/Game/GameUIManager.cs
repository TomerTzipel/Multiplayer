using Fusion;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionCanvas;
    [SerializeField] private GameObject gameCanvas;

    [SerializeField] private GameObject ScoreboardPanel;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text blueScoreText;

    [SerializeField] private PlayerScoreHandler[] redTeamScoreHandler;
    [SerializeField] private PlayerScoreHandler[] blueTeamScoreHandler;

    private void Awake()
    {
        gameCanvas.SetActive(false);
        ScoreboardPanel.SetActive(false);
        UpdateTime(0,0);
        UpdateScore(Team.Red, 0);
        UpdateScore(Team.Blue, 0);
    }

    public void SetUpGameUI()
    {
        selectionCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    public void AddPlayerToScoreboard(string name,Sprite splash,Team team,int playerTeamIndex)
    {
        if (team == Team.Red)
            redTeamScoreHandler[playerTeamIndex].InitializeUI(name, splash);
        if (team == Team.Blue)
            blueTeamScoreHandler[playerTeamIndex].InitializeUI(name, splash);
    }

    public void UpdateScoreboard(NetworkDictionary<NetworkString<_8>, ScoreData> playersScoreData)
    {
        foreach (var kvp in playersScoreData)
        {
            if (kvp.Value.Team == Team.Red)
            {
                foreach (var handler in redTeamScoreHandler)
                {
                    if(handler.OwnerName == kvp.Key)
                    {
                        handler.UpdateUI(kvp.Value.Kills, kvp.Value.Deaths);
                    }
                }
            }

            if (kvp.Value.Team == Team.Blue)
            {
                foreach (var handler in blueTeamScoreHandler)
                {
                    if (handler.OwnerName == kvp.Key)
                    {
                        handler.UpdateUI(kvp.Value.Kills, kvp.Value.Deaths);
                    }
                }
            }

        }
    }
    public void UpdateTime(int minutes,int secodns)
    {
        timerText.text = minutes.ToString("00") + ":" + secodns.ToString("00");
    }
    public void UpdateScore(Team team, int score)
    {
        if (team == Team.Red) redScoreText.text = score.ToString("00");
        if (team == Team.Blue) blueScoreText.text = score.ToString("00");
    }


}
