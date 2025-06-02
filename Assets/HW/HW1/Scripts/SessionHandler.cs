using Fusion;
using TMPro;
using UnityEngine;

public class SessionHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text playersCount;

    private SessionInfo _sessionInfo;
    private LobbyManager _lobbyManager;
    public void InitialzieData(SessionInfo sessionInfo,LobbyManager lobbyManger)
    {
        _lobbyManager = lobbyManger;
        _sessionInfo = sessionInfo;
        sessionName.text = sessionInfo.Name;
        playersCount.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
    }

    public void JoinSession()
    {
        _lobbyManager.JoinSession(_sessionInfo.Name);
    }
}
