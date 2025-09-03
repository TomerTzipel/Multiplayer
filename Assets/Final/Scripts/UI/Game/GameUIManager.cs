using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionCanvas;
    [SerializeField] private GameObject gameCanvas;

    [SerializeField] private GameObject ScoreboardPanel;

    [SerializeField] private PlayerScoreHandler[] redTeamScoreHandler;
    [SerializeField] private PlayerScoreHandler[] blueTeamScoreHandler;

    private void Awake()
    {
        gameCanvas.SetActive(false);
    }

    public void SetUpGameUI()
    {
        selectionCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }


}
