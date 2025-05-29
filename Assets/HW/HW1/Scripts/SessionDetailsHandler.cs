using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SessionDetailsHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text playersCount;
    public LobbyManager Manager { get; set; }
    public void InitialzieData(SessionInfo sessionInfo)
    {
        sessionName.text = sessionInfo.Name;
        playersCount.text = sessionInfo.PlayerCount.ToString();
    }

    public void JoinSessin()
    {
        Manager.JoinSession(sessionName.text);
    }
}
