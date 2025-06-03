using System.Collections.Generic;
using Fusion;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;


public class SessionUiManager : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager;
    
    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text userCount;
    
    [SerializeField] private Transform usersScrollViewContent;
    [SerializeField] private PlayerSessionDetails playerSessionDetailsPrefab;
    
    private List<PlayerSessionDetails> _playersDetails = new List<PlayerSessionDetails>();
    
    private void Awake()
    {
        HideSessionPanel();
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
    
    public void UpdateSessionUserCount(List<PlayerRef> playerList, NetworkRunner runner)
    {
        userCount.text = $"Users: {runner.SessionInfo.PlayerCount}/{runner.SessionInfo.MaxPlayers}";
        
        RemovePriorUsersList(playerList);
        
        PlayerSessionDetails currentPlayer;
        for (int i = 0; i < playerList.Count; i++)
        {
            // A new Player sessions Details needs to be created
            if(_playersDetails.Count <= i)
            {
                currentPlayer = Instantiate(playerSessionDetailsPrefab, usersScrollViewContent);
                currentPlayer.transform.localScale = Vector3.one;
                _playersDetails.Add(currentPlayer);
            }
            // A player sessions handler is already made and just need to update the data
            else
            {
                currentPlayer = _playersDetails[i];
            }
            currentPlayer.InitializeData(playerList[i]);
        }
    }

    private void RemovePriorUsersList(List<PlayerRef> playerList)
    {
        int sessionsDiff = _playersDetails.Count - playerList.Count;
        
        PlayerSessionDetails currentPlayer;
        int originalCount = _playersDetails.Count;
        for (int i = 0; i < sessionsDiff; i++)
        {
            currentPlayer = _playersDetails[originalCount - 1 - i];
            _playersDetails.Remove(currentPlayer);
            Destroy(currentPlayer.gameObject);
        }
    }
}