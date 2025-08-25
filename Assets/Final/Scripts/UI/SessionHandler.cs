using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SessionHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text playersCount;
    [SerializeField] private Button joinButton;

    private string _sessionName;

    public event UnityAction<string> OnJoinSession;

    public void InitialzieData(SessionInfo sessionInfo)
    {
        _sessionName = sessionInfo.Name;
        sessionName.text = sessionInfo.Name;
        sessionName.color = Color.black;
        playersCount.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";

        EnableButton(true);
    }

    public void EnableButton(bool value)
    {
        joinButton.interactable = value;
    }

    public void OnJoinSessionClicked()
    {
        OnJoinSession.Invoke(_sessionName);
    }

    public void LockSession()
    {
        EnableButton(false);
        sessionName.color = Color.red;
    }
}
