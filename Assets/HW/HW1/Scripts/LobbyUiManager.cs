using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] private GameObject lobbiesPanel;
    [SerializeField] private GameObject lobbyPanel; 
    
    [SerializeField] private TMP_Text lobbyTitle;
    [SerializeField] private TMP_Text lobbySessionsCount;

    [SerializeField] private SessionHandler sessionDetailsPrefab;
    [SerializeField] private Transform sessoionsScrollViewContent;

    private List<SessionHandler> _sessionHandlers = new List<SessionHandler>(4);

    void Awake()
    {
        ShowLobbiesList();
    }
    public void ShowLobbiesList()
    {
        lobbyPanel.SetActive(false);
        lobbiesPanel.SetActive(true);
    }
    public void ShowLobbySessionsList()
    {
        lobbyPanel.SetActive(true);
        lobbiesPanel.SetActive(false);
    }
    public void HideLobbyPanels()
    {
        lobbyPanel.SetActive(false);
        lobbiesPanel.SetActive(false);
    }
    public void UpdateLobbySessionsList(LobbyManager lobbyManager, List<SessionInfo> lobbySessions, NetworkRunner runner)
    {
        lobbyTitle.text = runner.LobbyInfo.Name + " Lobby";
        lobbySessionsCount.text = lobbySessions.Count.ToString();

        RemovePriorSessionsList(lobbySessions);
        if (lobbySessions.Count > 0)
        {
            SessionHandler curretnHandler;
            for (int i = 0; i < lobbySessions.Count; i++)
            {
                // A new sessions Handler needs to be created
                if(_sessionHandlers.Count <= i)
                {
                    curretnHandler = Instantiate(sessionDetailsPrefab, sessoionsScrollViewContent);
                    curretnHandler.transform.localScale = Vector3.one;
                    _sessionHandlers.Add(curretnHandler);
                }
                // A sessions handler is already made and just need to update the data
                else
                {
                    curretnHandler = _sessionHandlers[i];
                }
                curretnHandler.InitialzieData(lobbySessions[i],lobbyManager);
            }   
        }
    }

    private void RemovePriorSessionsList(List<SessionInfo> newLobbySessions)
    {
        int sessionsDiff = _sessionHandlers.Count - newLobbySessions.Count;

        //Delete Leftover Sessions (as in any sessions that would be over the new list count)
        SessionHandler curretnHandler;
        for (int i = 0; i < sessionsDiff; i++)
        {
            curretnHandler = _sessionHandlers[_sessionHandlers.Count - 1 - i];
            _sessionHandlers.Remove(curretnHandler);
            Destroy(curretnHandler.gameObject);
        }
    }
}
