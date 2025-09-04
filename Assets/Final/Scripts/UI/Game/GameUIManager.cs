using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameNetworkManager gameManager;
    [SerializeField] private CharactersRef charactersRef;

    [SerializeField] private GameObject selectionCanvas;
    [SerializeField] private GameObject gameCanvas;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject ScoreboardPanel;

    [SerializeField] private TMP_Text gameResultText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text blueScoreText;

    [SerializeField] private PlayerScoreHandler[] redTeamScoreHandlers;
    [SerializeField] private PlayerScoreHandler[] blueTeamScoreHandlers;

    private InputSystem_Actions _inputSystemActions;

    private void Awake()
    {
        _inputSystemActions = new InputSystem_Actions();
        gameOverPanel.SetActive(false);
        gameCanvas.SetActive(false);
        ScoreboardPanel.SetActive(false);
        UpdateTime(0,0);
        UpdateScore(Team.Red, 0);
        UpdateScore(Team.Blue, 0);
    }

    private void OnEnable()
    {
        _inputSystemActions.Player.Scoreboard.Enable();
        _inputSystemActions.Player.Scoreboard.started += EnableScoreboard;
        _inputSystemActions.Player.Scoreboard.canceled += DisableScoreboard;
    }

    private void OnDisable()
    {
        _inputSystemActions.Player.Scoreboard.started -= EnableScoreboard;
        _inputSystemActions.Player.Scoreboard.canceled -= DisableScoreboard;
        _inputSystemActions.Player.Scoreboard.Disable();
    }

    public void SetUpGameUI()
    {
        selectionCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    public void UpdateScoreboard(NetworkDictionary<NetworkString<_8>, ScoreData> playersScoreData)
    {
        int redIndex = 0, blueIndex = 0;

        for (int i = 0; i < redTeamScoreHandlers.Length; i++)
        {
            redTeamScoreHandlers[i].gameObject.SetActive(false);
            blueTeamScoreHandlers[i].gameObject.SetActive(false);
        }

        foreach (var kvp in playersScoreData)
        {
            if (kvp.Value.Team == Team.Red)
            {
                redTeamScoreHandlers[redIndex].gameObject.SetActive(true);
                redTeamScoreHandlers[redIndex].UpdateUI((string)kvp.Key, charactersRef.GetCharacterSpriteAt(kvp.Value.charachterIndex), kvp.Value.Kills, kvp.Value.Deaths);
                redIndex++;
            }

            if (kvp.Value.Team == Team.Blue)
            {
                blueTeamScoreHandlers[redIndex].gameObject.SetActive(true);
                blueTeamScoreHandlers[redIndex].UpdateUI((string)kvp.Key, charactersRef.GetCharacterSpriteAt(kvp.Value.charachterIndex), kvp.Value.Kills, kvp.Value.Deaths);
                blueIndex++;
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

    public void ActivateGameOverPanel(Team winningTeam,Team localPlayerTeam)
    {
        gameOverPanel.SetActive(true);
        ScoreboardPanel.SetActive(true);

        if (localPlayerTeam == Team.Spectator)
        {
            if (winningTeam == Team.Red)
            {
                gameResultText.text = "RED WON!";
                gameResultText.color = Color.red;
            }
            else
            {
                gameResultText.text = "BLUE WON!";
                gameResultText.color = Color.blue;
            }
            return;
        }

        UpdateGameResultText(winningTeam == localPlayerTeam);
    }

    private void UpdateGameResultText(bool result)
    {
        if (result)
        {
            gameResultText.text = "VICTORY!";
            gameResultText.color = Color.blue;
        }
        else
        {
            gameResultText.text = "DEFEAT!";
            gameResultText.color = Color.red;
        }
    }

    public void LeaveGame()
    {
        gameManager.Runner.Shutdown();
        SceneManager.LoadScene("MainMenuScene");
    }

    private void EnableScoreboard(InputAction.CallbackContext _)
    {
        if (!gameManager.IsGameRunning) return;
        ScoreboardPanel.SetActive(true);
    }
    private void DisableScoreboard(InputAction.CallbackContext _)
    {
        ScoreboardPanel.SetActive(false);
    }
}
