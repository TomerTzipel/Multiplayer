using Fusion;
using TMPro;
using UnityEngine;


public class SessionUiManager : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager;
    
    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text userCount;
    
    [SerializeField] private Transform usersScrollViewContent;
    private void Awake()
    {
        sessionPanel.SetActive(false);
    }
    public void ShowSessionPanel(NetworkRunner runner)
    {
        sessionPanel.SetActive(true);
        
        sessionName.text = runner.SessionInfo.Name;
        UpdateSessionUserCount(runner);
    }

    public void HideSessionPanel()
    {
        sessionPanel.SetActive(false);
    }

    public void UpdateSessionUserCount(NetworkRunner runner)
    {
        userCount.text = $"Users: {runner.SessionInfo.PlayerCount}/{runner.SessionInfo.MaxPlayers}";
    }
}