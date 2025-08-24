using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject sessionsPanel;
    [SerializeField] private GameObject createSessionPanel;
    [SerializeField] private GameObject directJoinPanel;

    private void Awake()
    {
        DisableAllElements();
        mainMenuPanel.SetActive(true);
    }

    public void EnableAllButtons(bool value)
    {
        foreach (var button in buttons)
        {
            button.interactable = value;
        }
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
