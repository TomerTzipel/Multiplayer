using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace HW3
{
    public class LobbyUiManager : MonoBehaviour
    {
        [SerializeField] private GameObject lobbiesPanel;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject sessionCreationPanel;

        [SerializeField] private TMP_Text lobbyTitle;
        [SerializeField] private TMP_Text lobbySessionsCount;

        [SerializeField] private SessionHandler sessionDetailsPrefab;
        [SerializeField] private Transform sessoionsScrollViewContent;

        [SerializeField] private TMP_InputField sessionNameField;
        [SerializeField] private Toggle sessionInvisibleToggle;

        private List<SessionHandler> _sessionHandlers = new List<SessionHandler>(4);
        private NetworkManager _networkManager;

        void Awake()
        {
            ShowLobbiesList();
            _networkManager = NetworkManager.Instance;
        }
        private void OnEnable()
        {
            _networkManager.OnJoinLobby += HandleJoinLobby;
            _networkManager.OnSessionListUpdate += UpdateLobbySessionsList;
        }

        private void OnDisable()
        {
            _networkManager.OnJoinLobby -= HandleJoinLobby;
            _networkManager.OnSessionListUpdate -= UpdateLobbySessionsList;
        }

        #region USED BY BUTTONS
        public void CreateNewSession()
        {
            if (sessionNameField.text.IsNullOrEmpty()) return;

            string sessionName = sessionNameField.text;

            NetworkManager.Instance.JoinSession(sessionName, sessionInvisibleToggle.isOn);
        }
        public void ShowLobbiesList()
        {
            lobbyPanel.SetActive(false);
            lobbiesPanel.SetActive(true);
        }
        public void JoinLobbyClicked(string name)
        {
            _networkManager.JoinLobby(name);
        }
        #endregion

        public void UpdateLobbySessionsList(List<SessionInfo> lobbySessions)
        {
           
            lobbySessionsCount.text = lobbySessions.Count.ToString();

            RemovePriorSessionsList(lobbySessions);
            if (lobbySessions.Count > 0)
            {
                SessionHandler curretnHandler;
                for (int i = 0; i < lobbySessions.Count; i++)
                {
                    // A new sessions Handler needs to be created
                    if (_sessionHandlers.Count <= i)
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
                    curretnHandler.InitialzieData(lobbySessions[i]);
                }
            }
        }
       
        private void HandleJoinLobby()
        {
            lobbyTitle.text = _networkManager.NetworkRunner.LobbyInfo.Name + " Lobby";
            lobbyPanel.SetActive(true);
            lobbiesPanel.SetActive(false);
        }
        private void RemovePriorSessionsList(List<SessionInfo> newLobbySessions)
        {
            int sessionsDiff = _sessionHandlers.Count - newLobbySessions.Count;

            //Delete Leftover Sessions (as in any sessions that would be over the new list count)
            SessionHandler curretnHandler;
            int originalCount = _sessionHandlers.Count;
            for (int i = 0; i < sessionsDiff; i++)
            {
                curretnHandler = _sessionHandlers[originalCount - 1 - i];
                _sessionHandlers.Remove(curretnHandler);
                Destroy(curretnHandler.gameObject);
            }
        }


    }

}
