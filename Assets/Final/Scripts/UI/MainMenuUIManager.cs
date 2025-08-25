using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private MainMenuNetworkManager networkManager;

    [SerializeField] private Button[] buttons;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject sessionsPanel;
    [SerializeField] private GameObject createSessionPanel;
    [SerializeField] private GameObject directJoinPanel;

    [SerializeField] private SessionsUIHandler sessionsManager;

    private void Awake()
    {
        DisableAllElements();
        mainMenuPanel.SetActive(true);
    }
    
    public void UpdateSessions(List<SessionInfo> sessions)
    {
        sessionsManager.UpdateSessionsList(sessions);
    }

    public void EnableAllButtons(bool value)
    {
        foreach (var button in buttons)
        {
            button.interactable = value;
        }
        sessionsManager.EnableAllButtons(value);
    }

    public void DisableAllElements()
    {
        mainMenuPanel.SetActive(false);
        sessionsPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        directJoinPanel.SetActive(false);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
