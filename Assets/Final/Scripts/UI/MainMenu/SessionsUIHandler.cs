using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SessionsUIHandler : MonoBehaviour 
{
    [SerializeField] private MainMenuNetworkManager networkManager;

    [SerializeField] private SessionHandler sessionHandlerPrefab;
    [SerializeField] private Transform sessoionsScrollViewContent;
    [SerializeField] private TMP_Text sessionsCountText;

    private List<SessionHandler> _sessionHandlers = new List<SessionHandler>(4);
    public void EnableAllButtons(bool value)
    {
        foreach (SessionHandler handler in _sessionHandlers)
        {
            handler.EnableButton(value);
        }
    }

    public void HandleJoinSession(string name)
    {
        networkManager.JoinSession(name);
    }

    public void UpdateSessionsList(List<SessionInfo> lobbySessions)
    {

        sessionsCountText.text = lobbySessions.Count.ToString();

        RemovePriorSessionsList(lobbySessions);
        if (lobbySessions.Count > 0)
        {
            SessionHandler currentHandler;
            for (int i = 0; i < lobbySessions.Count; i++)
            {
                // A new sessions Handler needs to be created
                if (_sessionHandlers.Count <= i)
                {
                    currentHandler = GameObject.Instantiate(sessionHandlerPrefab, sessoionsScrollViewContent);
                    currentHandler.transform.localScale = Vector3.one;
                    currentHandler.OnJoinSession += HandleJoinSession;
                    _sessionHandlers.Add(currentHandler);
                }
                // A sessions handler is already made and just need to update the data
                else
                {
                    currentHandler = _sessionHandlers[i];
                }
                currentHandler.InitialzieData(lobbySessions[i]);

                if (!lobbySessions[i].IsOpen) currentHandler.LockSession();
            }
        }
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
            GameObject.Destroy(curretnHandler.gameObject);
        }
    }
}
