using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class SessionUiManager : MonoBehaviour
{
    [SerializeField] private LobbyManager lobyManager;
    
    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text userCount;
    
    [SerializeField] private Transform usersScrollViewContent;

    public void ShowSessionPanel(NetworkRunner runner)
    {
        sessionPanel.SetActive(true);
        
        sessionName.text = runner.SessionInfo.Name;
        UpdateSessionUserCount(runner);
    }

    public void UpdateSessionUserCount(NetworkRunner runner)
    {
        userCount.text = $"Users: {runner.SessionInfo.PlayerCount}/{runner.SessionInfo.MaxPlayers}";
    }
}